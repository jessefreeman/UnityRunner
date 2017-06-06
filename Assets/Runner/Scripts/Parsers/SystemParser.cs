using PixelVisionSDK;

namespace PixelVisionRunner.Parsers
{
    public class SystemParser: JsonParser
    {
        protected ILoad target;

        public SystemParser(string jsonString, ILoad target) : base(jsonString)
        {
            this.target = target;
        }

        public override void CalculateSteps()
        {
            base.CalculateSteps();
            steps.Add(ApplySettings);

        }

        public virtual void ApplySettings()
        {

            //TODO need to loop through and pull out each chip (without pack
            //Debug.Log("Applying Settings");
            if (target != null)
                target.DeserializeData(data);
            currentStep++;
        }
    }
}
