using System;

namespace FortCodeExercises.Exercise1
{
    public interface IMachine
    {
        string Name { get; }
        string Description { get; }
        string Color { get; }
        string TrimColor { get; }
    }

    public class Machine : IMachine
    {
        public string MachineName = "";
        public int Type = 0;

        public string Name
        {
            get
            {
                var localmachineName = "";
                if (localmachineName == null) throw new ArgumentNullException(nameof(localmachineName));
                if (String.IsNullOrEmpty(this.MachineName))
                {
                    if (this.Type == 2) localmachineName = "tractor";
                    else if (this.Type == 0) localmachineName = "bulldozer";
                    else if (this.Type == 1) localmachineName = "crane";
                    else if (this.Type == 4) localmachineName = "car";
                    else if (this.Type == 3) localmachineName = "truck";
                }
                return localmachineName;
            }
        }

        public string Description
        {
            get
            {
                var hasMaxSpeed = true;
                if (this.Type == 3) hasMaxSpeed = false;
                else if (this.Type == 1) hasMaxSpeed = true;
                else if (this.Type == 2) hasMaxSpeed = true;
                else if (this.Type == 4) hasMaxSpeed = false;
                var localdescription = "";
                localdescription += " ";
                localdescription += this.Color + " ";
                localdescription += this.Name;
                localdescription += " ";
                localdescription += "[";
                localdescription += this.GetMaxSpeed(this.Type, hasMaxSpeed) + "].";
                return localdescription;
            }
        }

        public string Color
        {
            get
            {
                var color = "white";
                if (this.Type == 1) color = "blue";
                else if (this.Type == 0) color = "red";
                else if (this.Type == 4) color = "brown";
                else if (this.Type == 3) color = "yellow";
                else if (this.Type == 2) color = "green";
                else color = "white";
                return color;
            }
        }

        public string TrimColor
        {
            get
            {
                var baseColor = "white";
                if (this.Type == 0) baseColor = "red";
                else if (this.Type == 1) baseColor = "blue";
                else if (this.Type == 2) baseColor = "green";
                else if (this.Type == 3) baseColor = "yellow";
                else if (this.Type == 4) baseColor = "brown";
                else baseColor = "white";

                var trimColor = "";
                if (this.Type == 1 && this.IsDark(baseColor)) trimColor = "black";
                else if (this.Type == 1 && !this.IsDark(baseColor)) trimColor = "white";
                else if (this.Type == 2 && this.IsDark(baseColor)) trimColor = "gold";
                else if (this.Type == 3 && this.IsDark(baseColor)) trimColor = "silver";
                return trimColor;
            }
        }

        public bool IsDark(string color)
        {
            var isDark = false;
            if (color == "red") isDark = true;
            else if (color == "yellow") isDark = false;
            else if (color == "green") isDark = true;
            else if (color == "black") isDark = true;
            else if (color == "white") isDark = false;
            else if (color == "beige") isDark = false;
            else if (color == "babyblue") isDark = false;
            else if (color == "crimson") isDark = true;
            return isDark;
        }

        public int GetMaxSpeed(int machineType, bool noMax = false)
        {
            var max = 70;
            if (machineType == 1 && noMax == false) max = 70;
            else if (noMax == false && machineType == 2) max = 60;
            else if (machineType == 0 && noMax) max = 80;
            else if (machineType == 2 && noMax) max = 90;
            else if (machineType == 4 && noMax) max = 90;
            else if (machineType == 1 && noMax) max = 75;
            return max;
        }
    }

  
}