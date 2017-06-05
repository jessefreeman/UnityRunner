using System;
using System.Collections.Generic;

namespace PixelVisionRunner.Parsers
{
    public abstract class AbstractParser
    {
        
        public int currentStep { get; protected set; }
        public int totalSteps { get { return steps.Count; } }

        protected List<Action> steps = new List<Action>();

        public bool completed
        {
            get
            {
                return currentStep >= totalSteps;
            }
        }
       
        public virtual void CalculateSteps()
        {
            currentStep = 0;
        }

        public virtual void NextStep()
        {
            if (completed)
                return;

            //Debug.Log("Next Step "+ currentStep +" of "+ totalSteps);

            steps[currentStep]();
        }

    }
}
