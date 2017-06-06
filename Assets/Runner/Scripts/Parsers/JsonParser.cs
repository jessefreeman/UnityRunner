//  
// Copyright (c) Jesse Freeman. All rights reserved.  
// 
// Licensed under the Microsoft Public License (MS-PL) License. 
// See LICENSE file in the project root for full license information. 
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany
// 

using System.Collections.Generic;
using MiniJSON;
using PixelVisionSDK;
using Object = System.Object;

namespace PixelVisionRunner.Parsers
{

    public class JsonParser: AbstractParser
    {

        protected string jsonString;
        protected Dictionary<string, Object> data;

        public JsonParser(string jsonString)
        {
            //Debug.Log("New Json Parser");
            this.jsonString = jsonString;

            CalculateSteps();
        }

        public override void CalculateSteps()
        {
            base.CalculateSteps();
            steps.Add(ParseJson);

            //Debug.Log("Calculating "+totalSteps+" steps.");
        }

        public virtual void ParseJson()
        {
            //Debug.Log("Parsing Json");
            data = Json.Deserialize(jsonString) as Dictionary<string, object>;
            currentStep++;
        }

//        public virtual void ApplySettings()
//        {
//            //Debug.Log("Applying Settings");
//            if(target != null)
//                target.DeserializeData(data);
//            currentStep++;
//        }
    }

}