﻿using SubterfugeCore.Core.Entities.Specialists;
using SubterfugeCore.Core.GameEvents.Validators;
using SubterfugeCore.Core.Interfaces.Outpost;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubterfugeCore.Core.GameEvents.ReversibleEvents
{
    public class SpecialistCombat : IReversible
    {
        ICombatable combatant1;
        ICombatable combatant2;
        bool eventSuccess = false;

        List<Specialist> combatant1Specialists = new List<Specialist>();
        List<Specialist> combatant2Specialists = new List<Specialist>();

        public SpecialistCombat(ICombatable combatant1, ICombatable combatant2)
        {
            this.combatant1 = combatant1;
            this.combatant2 = combatant2;

        }
        public bool backwardAction()
        {
            if (!eventSuccess)
            {
                return false;
            }

            List<Specialist> specialists = new List<Specialist>();
            specialists.AddRange(combatant1Specialists);
            specialists.AddRange(combatant2Specialists);

            while (specialists.Count > 0)
            {
                Specialist lowPriority = null;
                foreach (Specialist s in specialists)
                {
                    if (lowPriority == null || s.getPriority() >= lowPriority.getPriority())
                    {
                        lowPriority = s;
                    }
                }
                // Apply the specialist effect to the enemey.
                ICombatable enemy = combatant1.getOwner() == lowPriority.getOwner() ? combatant2 : combatant1;
                lowPriority.undoEffect(enemy);
            }
            return true;
        }

        public bool forwardAction()
        {
            this.combatant1Specialists = combatant1.getSpecialistManager().getSpecialists();
            this.combatant2Specialists = combatant1.getSpecialistManager().getSpecialists();

            List<Specialist> specialists = new List<Specialist>();
            specialists.AddRange(combatant1.getSpecialistManager().getSpecialists());
            specialists.AddRange(combatant2.getSpecialistManager().getSpecialists());

            while (specialists.Count > 0)
            {
                Specialist topPriority = null;
                foreach (Specialist s in specialists)
                {
                    // If any of the specialists are invalid, cancel the event.
                    if (!Validator.validateSpecialist(s))
                    {
                        this.eventSuccess = false;
                        return false;
                    }
                    if (topPriority == null || s.getPriority() < topPriority.getPriority())
                    {
                        topPriority = s;
                    }
                }
                // Apply the specialist effect to the enemey.
                ICombatable enemy = combatant1.getOwner() == topPriority.getOwner() ? combatant2 : combatant1;
                topPriority.applyEffect(enemy);
            }
            return true;
        }
    }
}
