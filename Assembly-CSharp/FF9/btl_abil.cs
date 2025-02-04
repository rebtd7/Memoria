﻿using System;
using Memoria;
using Memoria.Data;
using UnityEngine;

namespace FF9
{
	public class btl_abil
	{
		public static Boolean CheckPartyAbility(UInt32 sa_buf_no, UInt32 sa_bit)
		{
			for (Int32 i = 0; i < 4; i++)
			{
				PLAYER player = FF9StateSystem.Common.FF9.party.member[i];
				if (player != null && (player.sa[(Int32)((UIntPtr)sa_buf_no)] & sa_bit) != 0u)
				{
					return true;
				}
			}
			return false;
		}

		public static Boolean TryReturnMagic(BattleUnit returner, BattleUnit originalCaster, BattleCommand command)
		{
			if (returner.IsUnderAnyStatus(BattleStatus.Petrify | BattleStatus.Venom | BattleStatus.Stop | BattleStatus.Sleep | BattleStatus.Freeze | BattleStatus.Death) || FF9StateSystem.Battle.FF9Battle.btl_phase != 4)
				return false;
			BattleCommandId cmdId = originalCaster.IsPlayer ? BattleCommandId.Counter : BattleCommandId.MagicCounter;
			if (Configuration.Battle.CountersBetterTarget)
			{
				if (command.Data.tar_id == returner.Id) // Single-target magic
				{
					if (originalCaster.IsUnderStatus(BattleStatus.Death))
					{
						UInt16 retarget_id = (UInt16)(originalCaster.IsPlayer ? 1 : 16);
						for (Int32 i = 0; i < 4; i++)
						{
							BattleUnit new_target = btl_scrp.FindBattleUnit((UInt16)(retarget_id << i));
							if (new_target != null && new_target.Data.bi.target != 0 && !new_target.IsUnderStatus(BattleStatus.Death))
							{
								btl_cmd.SetCounter(returner.Data, cmdId, (Int32)command.Data.sub_no, new_target.Id);
								return true;
							}
						}
						return false;
					}
					else
						btl_cmd.SetCounter(returner.Data, cmdId, (int)command.Data.sub_no, originalCaster.Id);
				}
				else if ((command.Data.tar_id & 0xF) != 0 && (command.Data.tar_id & 0xF0) != 0) // Most likely targeting everyone
					btl_cmd.SetCounter(returner.Data, cmdId, (int)command.Data.sub_no, btl_scrp.GetBattleID(2u));
				else // Multi-target magic
					btl_cmd.SetCounter(returner.Data, cmdId, (int)command.Data.sub_no, btl_scrp.GetBattleID(originalCaster.IsPlayer ? 0u : 1u));
			}
			else
			{
				btl_cmd.SetCounter(returner.Data, cmdId, (int)command.Data.sub_no, originalCaster.Id);
			}
			return true;
		}

	    public static Boolean CheckCounterAbility(BattleTarget defender, BattleCaster attacker, BattleCommand command)
		{
			// Dummied
			if (defender.IsUnderStatus(BattleStatus.NoReaction) || command.Id > BattleCommandId.EnemyAtk)
                return false;

            if (defender.HasSupportAbility(SupportAbility2.Counter) && (command.AbilityCategory & 8) != 0) // Physical
            {

                Int32 chance = defender.Will;
	            if (defender.HasSupportAbility(SupportAbility2.Eye4Eye))
	                chance *= 2;

                if (chance > Comn.random16() % 100)
	            {
	                btl_cmd.SetCounter(defender.Data, BattleCommandId.Counter, 176, attacker.Id);
	                return true;
	            }
	        }

			if (defender.HasSupportAbility(SupportAbility2.ReturnMagic) && (command.AbilityCategory & 128) != 0) // Magic
				return TryReturnMagic(defender, attacker, command);

			return false;
        }

	    public static void CheckAutoItemAbility(BattleTarget defender, BattleCommand command)
		{
			const Byte potion1Id = 236;
	        const Byte potion2Id = 237;
	        const Byte potionScriptId = 069;

	        if (!defender.HasSupportAbility(SupportAbility2.AutoPotion))
	            return;

	        if (defender.IsUnderStatus(BattleStatus.NoReaction) || command.Id > BattleCommandId.EnemyAtk)
	            return;

	        Int32 overhealLimit = Configuration.Battle.AutoPotionOverhealLimit;

            // Vanila
	        if (overhealLimit < 0)
	        {
	            foreach (Byte potionId in new[] {potion1Id, potion2Id})
	            {
	                if (ff9item.FF9Item_GetCount(potionId) != 0)
	                {
	                    UIManager.Battle.ItemRequest(potionId);
	                    btl_cmd.SetCounter(defender.Data, BattleCommandId.AutoPotion, potionId, defender.Id);
	                    break;
	                }
	            }
	        }
            // Better auto-potions
	        else
	        {
	            Byte betterPotionId = 0;

	            foreach (Byte potionId in new[] {potion1Id, potion2Id})
	            {
	                if (ff9item.FF9Item_GetCount(potionId) < 1)
	                    continue;

	                CMD_DATA testCommand = new CMD_DATA
	                {
	                    cmd_no = BattleCommandId.AutoPotion,
	                    sub_no = potionId,
	                    tar_id = defender.Id,
	                    info = new CMD_DATA.SELECT_INFO()
	                };
					testCommand.SetAAData(FF9StateSystem.Battle.FF9Battle.aa_data[0]);

					BattleCalculator v = new BattleCalculator(defender.Data, defender.Data, new BattleCommand(testCommand));
	                BattleScriptFactory factory = SBattleCalculator.FindScriptFactory(potionScriptId);
	                if (factory != null)
	                {
	                    IBattleScript script = factory(v);
	                    script.Perform();
	                }

	                Int32 heal = v.Target.HpDamage;
	                Int32 harm = (Int32)v.Target.MaximumHp - (Int32)v.Target.CurrentHp;

	                if (heal - harm > heal * overhealLimit / 100)
	                    break;

	                betterPotionId = potionId;
	            }

	            if (betterPotionId != 0)
	            {
	                UIManager.Battle.ItemRequest(betterPotionId);
	                btl_cmd.SetCounter(defender.Data, BattleCommandId.AutoPotion, betterPotionId, defender.Id);
	            }
	        }
	    }

	    public static UInt16 CheckCoverAbility(UInt16 tar_id, Boolean[] candidates)
	    {
	        BattleUnit coverBy = null;
	        BattleUnit targetUnit = btl_scrp.FindBattleUnit(tar_id);

	        coverBy = FindStrongestDefender(candidates, targetUnit);

	        if (coverBy == null)
	            return 0;

	        coverBy.FaceTheEnemy();
	        coverBy.Data.pos[0] = targetUnit.Data.pos[0];
	        coverBy.Data.pos[2] = targetUnit.Data.pos[2];

	        targetUnit.Data.pos[2] -= 400f;

	        btl_mot.setMotion(coverBy.Data, BattlePlayerCharacter.PlayerMotionIndex.MP_COVER);
	        coverBy.IsCovered = true;

            return coverBy.Id;
	    }

	    private static BattleUnit FindStrongestDefender(Boolean[] candidates, BattleUnit targetUnit)
	    {
	        BattleUnit coverBy = null;
	        foreach (BattleUnit next in FF9StateSystem.Battle.FF9Battle.EnumerateBattleUnits())
	        {
	            if (!candidates[Comn.firstBitSet(next.Id)] || next.Id == targetUnit.Id)
	                continue;

	            if (coverBy == null || coverBy.CurrentHp < next.CurrentHp)
	                coverBy = next;
	        }
	        return coverBy;
	    }

	    public static void CheckReactionAbility(BTL_DATA btl, AA_DATA aa)
		{
			// Dummied
			if (!Status.checkCurStat(btl, BattleStatus.NoReaction))
			{
				if ((btl.sa[1] & 1048576u) != 0u && btl.cur.hp != 0 && Status.checkCurStat(btl, BattleStatus.LowHP))
				{
					if (btl.cur.hp + btl.max.hp / 2 < btl.max.hp)
					{
						btl.cur.hp += btl.max.hp / 2;
					}
					else
					{
						btl.cur.hp = btl.max.hp;
					}
				}
				if ((btl.sa[1] & 8388608u) != 0u && aa.MP != 0)
				{
					if (btl.cur.mp + aa.MP < btl.max.mp)
					{
						btl.cur.mp += aa.MP;
					}
					else
					{
						btl.cur.mp = btl.max.mp;
					}
				}
			}
		}

		public static void CheckStatusAbility(BattleUnit btl)
		{
			foreach (SupportingAbilityFeature saFeature in ff9abil.GetEnabledSA(btl.Data.sa))
				saFeature.TriggerOnStatusInit(btl);

			/*if (btl.HasSupportAbility(SupportAbility1.AutoReflect))
            {
                btl.PermanentStatus |= BattleStatus.Reflect;
//				HonoluluBattleMain.battleSPS.AddBtlSPSObj(btl, BattleStatus.Reflect);
			}

			if (btl.HasSupportAbility(SupportAbility1.AutoFloat))
			    btl.PermanentStatus |= BattleStatus.Float;

            if (btl.HasSupportAbility(SupportAbility1.AutoHaste))
            {
                btl.PermanentStatus |= BattleStatus.Haste;
                btl.ResistStatus |= BattleStatus.Slow;
//				btl.Data.cur.at_coef = (SByte)(btl.Data.cur.at_coef * 3 / 2);
//				HonoluluBattleMain.battleSPS.AddBtlSPSObj(btl, BattleStatus.Haste);
			}

            if (btl.HasSupportAbility(SupportAbility1.AutoRegen))
            {
                btl.PermanentStatus |= BattleStatus.Regen;
//				btl_stat.SetOprStatusCount(btl.Data, 18u);
			}

            if (btl.HasSupportAbility(SupportAbility1.AutoLife))
                btl.CurrentStatus |= BattleStatus.AutoLife;

            if (btl.HasSupportAbility(SupportAbility2.BodyTemp))
                btl.ResistStatus |= (BattleStatus.Freeze | BattleStatus.Heat);

            if (btl.HasSupportAbility(SupportAbility2.Insomniac))
                btl.ResistStatus |= BattleStatus.Sleep;

            if (btl.HasSupportAbility(SupportAbility2.Antibody))
                btl.ResistStatus |= (BattleStatus.Poison | BattleStatus.Venom);

            if (btl.HasSupportAbility(SupportAbility2.BrightEyes))
                btl.ResistStatus |= BattleStatus.Blind;

            if (btl.HasSupportAbility(SupportAbility2.Loudmouth))
			    btl.ResistStatus |= BattleStatus.Silence;

            if (btl.HasSupportAbility(SupportAbility2.Jelly))
                btl.ResistStatus |= (BattleStatus.Petrify | BattleStatus.GradualPetrify);

		    if (btl.HasSupportAbility(SupportAbility2.Locomotion))
		        btl.ResistStatus |= BattleStatus.Stop;

		    if (btl.HasSupportAbility(SupportAbility2.ClearHeaded))
		        btl.ResistStatus |= BattleStatus.Confuse;*/
		}
	}
}
