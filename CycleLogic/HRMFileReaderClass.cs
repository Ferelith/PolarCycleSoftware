﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogicForCycle
{
    public class HRMFileReaderClass
    { 
        
        // Private variables
        private double duration = 0;
        private DateTime startDateTime;
        private int[] smode = new int[8];
        private int frequency = 0, startYear = 0, startMonth = 0, startDay = 0;
        private int[] heartRate, speed, cadence, altitude, power, pedallingIndex, lrbalanceLeftLeg;
        private string startTimeText, durationText;
        private DateTime startDate, durationDateTime;
        private string speedMeasurementUnit = "mph";
        private string distanceMeasurementUnit = "miles";

        public HRMFileReaderClass() { }

        public string get_fileName(string directory = @"C:\")
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open HRM File";
            dialog.Filter = "HRM files|*.hrm";
            dialog.InitialDirectory = directory;
            dialog.ShowDialog();
            return dialog.FileName;
        }
        public string[] open_file(string directory = @"C:\")
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open HRM File";
            dialog.Filter = "HRM files|*.hrm";
            dialog.InitialDirectory = directory;
            dialog.ShowDialog();
            string[] line = File.ReadAllLines(dialog.FileName);
            return line;
        }

        public string[] open_specific_file(string path)
        {
            string[] line = File.ReadAllLines(path);
            return line;
        }

        // Loops through line[] one line at a time to get data
        
        public void read_file(string[] line)
        {

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Contains("SMode"))
                {
                    smode[0] = int.Parse(line[i].Substring(6, 1));
                    smode[1] = int.Parse(line[i].Substring(7, 1));
                    smode[2] = int.Parse(line[i].Substring(8, 1));
                    smode[3] = int.Parse(line[i].Substring(9, 1));
                    smode[4] = int.Parse(line[i].Substring(10, 1));
                    smode[5] = int.Parse(line[i].Substring(11, 1));
                    smode[6] = int.Parse(line[i].Substring(12, 1));
                }
                else if (line[i].Contains("Date"))
                {
                    startYear = int.Parse(line[i].Substring(5, 4));
                    startMonth = int.Parse(line[i].Substring(9, 2));
                    startDay = int.Parse(line[i].Substring(11, 2));
                }
                else if (line[i].Contains("StartTime"))
                    startTimeText = line[i].Substring(10, 8);
                else if (line[i].Contains("Interval"))
                    frequency = int.Parse(line[i].Substring(9));
                else if (line[i].Contains("Length"))
                    durationText = line[i].Substring(7, 8);
                else if (line[i].Contains("[HRData]"))
                {

                    // Time Formats
                    startDate = new DateTime(startYear, startMonth, startDay);
                    startDateTime = startDate + TimeSpan.Parse(startTimeText);
                    durationDateTime = Convert.ToDateTime(durationText);
                    duration = TimeSpan.Parse(durationText).TotalSeconds;

                    // Advance Line to HRData
                    i++;

                    // Loop Count for Arrays
                    int x = 0;

                    // Initiate Arrays for HRData
                    // Size is length of array minus current
                    // position in line array
                    heartRate = new int[line.Length - i];
                    speed = new int[line.Length - i];
                    cadence = new int[line.Length - i];
                    altitude = new int[line.Length - i];
                    power = new int[line.Length - i];
                    pedallingIndex = new int[line.Length - i];
                    lrbalanceLeftLeg = new int[line.Length - i];

                    // Store Data in Relevant Arrays
                    for (int j = i; j < line.Length; j++)
                    {
                        // Split each line of data
                        string[] HRDataLine = line[j].Split('\t');

                        // Split HRData Lines
                        if (smode[0] == 1) speed[x] = int.Parse(HRDataLine[1]) / 10;
                        if (smode[1] == 1) cadence[x] = int.Parse(HRDataLine[2]);
                        if (smode[2] == 1) altitude[x] = int.Parse(HRDataLine[3]);
                        if (smode[3] == 1) power[x] = int.Parse(HRDataLine[4]);

                        if (smode[4] == 1)
                        {
                            // Power Balance & Pedalling Index
                            ushort number = Convert.ToUInt16(HRDataLine[5]);
                            byte pIndex = (byte)(number & 1111111100000000 >> 8);
                            byte LRBleftLeg = (byte)(number & 0xff);
                            pedallingIndex[x] = pIndex;
                            lrbalanceLeftLeg[x] = LRBleftLeg;
                        }

                        if (smode[5] == 1)
                        {
                            speedMeasurementUnit = "km/h";
                            distanceMeasurementUnit = "km";
                        }
                        else
                        {
                            speedMeasurementUnit = "mph";
                            distanceMeasurementUnit = "miles";
                        }

                        if (smode[6] == 1) heartRate[x] = int.Parse(HRDataLine[0]);

                        x++;
                    }
                }
            }
        }

        // Properties storing parsed data - get only
        public int[] Speed
        {
            get { return speed; }
        }

        public int[] Cadence
        {
            get { return cadence; }
        }

        public int[] Altitude
        {
            get { return altitude; }
        }

        public int[] Power
        {
            get { return power; }
        }

        public int[] PedallingIndex
        {
            get { return pedallingIndex; }
        }

        public int[] LRBalanceLeftLeg
        {
            get { return lrbalanceLeftLeg; }
        }

        public int[] HeartRate
        {
            get { return heartRate; }
        }

        public DateTime StartDateTime
        {
            get { return startDateTime; }
        }

        public double DurationSeconds
        {
            get { return duration; }
        }

        public string SpeedMeasurementUnit
        {
            get { return speedMeasurementUnit; }
        }

        public string DistanceMeasurementUnit
        {
            get { return distanceMeasurementUnit; }
        }

        public int Frequency
        {
            get { return frequency; }
        }
    }
}
