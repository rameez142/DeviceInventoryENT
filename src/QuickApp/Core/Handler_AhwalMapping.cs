using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MOI.Patrol.Core
{
    public class Handler_AhwalMapping
    {
        public const int PatrolPersonState_None = 10;

        public const int PatrolPersonState_SunRise = 20;
        public const int PatrolPersonState_Sea = 30;
        public const int PatrolPersonState_Back = 40;
        public const int PatrolPersonState_BackFromWalking = 74;

        public const int PatrolPersonState_SunSet = 50;
        public const int PatrolPersonState_Away = 60;
        public const int PatrolPersonState_Land = 70;
        public const int PatrolPersonState_WalkingPatrol = 72;

        public const int PatrolPersonState_Absent = 80;
        public const int PatrolPersonState_Off = 90;
        public const int PatrolPersonState_Sick = 100;



        public const int CheckInState = 10;
        public const int CheckOutState = 20;


        //patrol roles
        public const int PatrolRole_None = 0;
        public const int PatrolRole_CaptainAllSectors = 10;
        public const int PatrolRole_CaptainShift = 20;
        public const int PatrolRole_CaptainSector = 30;
        public const int PatrolRole_SubCaptainSector = 40;
        public const int PatrolRole_CityGroupOfficer = 50;
        public const int PatrolRole_PatrolPerson = 60;
        public const int PatrolRole_Associate = 70;
        public const int PatrolRole_Temp = 80;
        //public const int PatrolRole_TodaysOff = 70;
        //public const int PatrolRole_Absent = 80;
        //public const int PatrolRole_SickLeave = 90;
        //shifts
        public const int Shifts_Morning = 1;
        public const int Shifts_Afternoon = 2;
        public const int Shifts_Night = 3;

        //sectors 
        public const int Sector_Public = 1;
        public const int Sector_First = 2;
        public const int Sector_Second = 3;
        public const int Sector_Third = 4;
        public const int Sector_Fourth = 5;
        public const int Sector_Fifth = 6;
        public const int Sector_North = 7;
        public const int Sector_South = 8;
        public const int Sector_West = 9;

    }
}
