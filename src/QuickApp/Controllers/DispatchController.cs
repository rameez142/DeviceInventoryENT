using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Npgsql;
using MOI.Patrol;
using MOI.Patrol.DataAccessLayer;
using MOI.Patrol.Core;
using Newtonsoft.Json.Linq;

namespace MOI.Patrol.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DispatchController : ControllerBase
    {
        public String constr = "server=localhost;Port=5432;User Id=postgres;password=admin;Database=Patrols";
        public DataAccess DAL = new DataAccess();
       
        /*AhwalMapping*/
        #region AhwalMapping
        [HttpGet("rolesList")]
        public List<Patrolroles> GetRolesList()
        {

            String Qry = "SELECT PatrolRoleID, Name FROM PatrolRoles";
            return DAL.PostGre_GetData<Patrolroles>(Qry);
        }

        [HttpGet("shiftsList")]
        public List<Shifts> GetShiftsList()
        {
            String Qry = "SELECT ShiftID, Name, StartingHour, NumberOfHours FROM Shifts";
            return DAL.PostGre_GetData<Shifts>(Qry);
        }

        [HttpGet("sectorsList")]
        public List<Sectors> GetSectorsList(int Userid)
        {
            String Qry = "SELECT SectorID, ShortName, CallerPrefix, Disabled,AhwalId FROM Sectors where Disabled<>1  and (AhwalID IN (SELECT AhwalID FROM UsersRolesMap WHERE (Userid = " + Userid + ") ))";
            return DAL.PostGre_GetData<Sectors>(Qry);
        }

        [HttpGet("cityList")]
        public List<Citygroups> GetCityList(int Userid, int sectorid)
        {
            String Qry = "SELECT CityGroupID ,  ShortName ,  CallerPrefix ,  Disabled ,AhwalID,SectorID,Text FROM  CityGroups  where Disabled<>1 and CallerPreFix<>'0' and SectorID=" + sectorid + " and  (AhwalID IN (SELECT AhwalID FROM UsersRolesMap WHERE (Userid = " + Userid + ")))";
            return DAL.PostGre_GetData<Citygroups>(Qry);
        }

        [HttpGet("associateList")]
        public List<Persons> GetAssociateList(int Userid)
        {
            String Qry = "SELECT AhwalMapping.AhwalMappingID, Persons.Personid, Persons.Milnumber, Persons.Name FROM AhwalMapping INNER JOIN Persons ON AhwalMapping.Personid = Persons.Personid WHERE (AhwalMapping.Patrolroleid <> 70) AND(AhwalMapping.Ahwalid IN (SELECT AhwalMapping.Ahwalid FROM UsersRolesMap WHERE (Userid = " + Userid + ") ))";
            return DAL.PostGre_GetData<Persons>(Qry);
        }

        
        public Ahwalmapping GetAhwal(int ahwalmappingid)
        {
            String Qry = "SELECT lastComeBackTimeStamp,lastAwayTimeStamp,incidentID,lastLandTimeStamp,hasFixedCallerID,handHeldID,sortingIndex,sunRiseTimeStamp,sunSetTimeStamp,patrolPersonStateID,hasDevices,callerID,cityGroupID,ahwalMappingID,sectorID,patrolRoleID," +
                "shiftID, Persons.Personid,Persons.Ahwalid, Persons.Milnumber, Persons.Name as personName,Persons.RankId  FROM AhwalMapping" +
                " INNER JOIN Persons ON AhwalMapping.Personid = Persons.Personid where AhwalMappingId =" + ahwalmappingid;
            List<Ahwalmapping> obj = DAL.PostGre_GetData<Ahwalmapping>(Qry);

            if (obj.Count > 0)
            {
                return obj[0];
            }
            return null;
           

        }

      
        [HttpGet("personForUserForRole")]
        public Persons GetPersonForUserForRole(int mno, int Userid)
        {

            String Qry = "SELECT Personid, AhwalId, Name, MilNumber,RankId,Mobile,FixedCallerId FROM Persons WHERE AhwalID IN (SELECT AhwalID FROM UsersRolesMap WHERE Userid = " + Userid + " ) and MilNumber = " + mno;
            List <Persons> obj = DAL.PostGre_GetData<Persons>(Qry);
            
            if(obj.Count > 0)
            {
                return obj[0];
            }
                return null;
        }
        public Persons GetPersonById(long Personid = -1)
        {

            String Qry = "SELECT Personid, AhwalId, Name, MilNumber,RankId,Mobile,FixedCallerId FROM Persons WHERE  Personid = " + Personid;
            List<Persons> obj = DAL.PostGre_GetData<Persons>(Qry);

            if (obj.Count > 0)
            {
                return obj[0];
            }
            return null;
        }

        [HttpGet("personsList")]
        public List<Persons> GetPersonsList(int Userid)
        {

                String Qry = "SELECT Personid, AhwalID, Name, MilNumber,RankId,Mobile,FixedCallerId FROM Persons WHERE AhwalID IN (SELECT AhwalID FROM UsersRolesMap WHERE Userid = " + Userid + " )";
            return DAL.PostGre_GetData<Persons>(Qry);
        }

        [HttpGet("dispatchList")]
        public DataTable Getdispatchlist()
        {


            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            DataTable dt = new DataTable();
            String Qry = "SELECT AhwalMappingID, AhwalID, ShiftID, SectorID, PatrolRoleID, CityGroupID,(Select MilNumber From Persons where Personid = AhwalMapping.Personid) as MilNumber,";
            Qry = Qry + " (Select RankID From Persons where Personid = AhwalMapping.Personid) as RankID, (Select Name From Persons where Personid = AhwalMapping.Personid) as PersonName, CallerID,  ";
            Qry = Qry + " HasDevices, '' as Serial,  (Select plateNumber From patrolcars where patrolid = AhwalMapping.patrolid) as PlateNumber, ";
            Qry = Qry + " PatrolPersonStateID, SunRiseTimeStamp, SunSetTimeStamp,(Select Mobile From Persons where Personid = AhwalMapping.Personid) as PersonMobile,IncidentID,";
            Qry = Qry + " LastStateChangeTimeStamp,(Select ShortName From sectors where SectorID=AhwalMapping.Sectorid) as SectorDesc , (Select (select Name from Ranks where rankid = persons.rankid) From Persons where Personid=AhwalMapping.Personid) as RankDesc,(SELECT  Name FROM PatrolPersonStates PS ";
            Qry = Qry + " where PS.Patrolpersonstateid in (select PatrolPersonStateID from PatrolPersonStateLog where PatrolPersonStateLog.Personid = AhwalMapping.Personid ";

            Qry = Qry + " order by TimeStamp desc  FETCH FIRST ROW ONLY ) ) as PersonState FROM AhwalMapping ";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(Qry, cont);
            da.Fill(dt);
            cont.Close();
            cont.Dispose();


            return dt;
        }

        [HttpPost("addAhwalMapping")]
        public Operationlogs PostAddAhwalMapping([FromBody]JObject data)
        {
            Ahwalmapping frm = Newtonsoft.Json.JsonConvert.DeserializeObject<Ahwalmapping>(data["ahwalmappingobj"].ToString(), new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore}); 
            Users u = data["userobj"].ToObject<Users>();
            //  GetPerson = "";
            // string ol_failed = "";
            //Operationlogs ol = new Operationlogs();
            //we have to check first that this person doesn't exists before in mapping
            Persons GetPerson = GetPersonById(frm.Personid);
            if (GetPerson == null)
            {
                Operationlogs ol_failed = new Operationlogs();
                ol_failed.Userid = u.Userid;
                ol_failed.Operationid = Handler_Operations.Opeartion_Mapping_AddNew;
                ol_failed.Statusid = Handler_Operations.Opeartion_Status_Failed;

                ol_failed.Text = "لم يتم العثور على الفرد: " + frm.Personid; //todo, change it actual person name
                Handler_Operations.Add_New_Operation_Log(ol_failed);
                return ol_failed;
            }

            string person_mapping_exists = DAL.PostGre_ExScalar("select count(1) from AhwalMapping where Personid = " + frm.Personid);
            if (person_mapping_exists == null || person_mapping_exists == "0")
            {
                Operationlogs ol_failed = new Operationlogs();
                ol_failed.Userid = u.Userid;
                ol_failed.Operationid = Handler_Operations.Opeartion_Mapping_AddNew;
                ol_failed.Statusid = Handler_Operations.Opeartion_Status_Failed;
                ol_failed.Text = "هذا الفرد موجود مسبقا، لايمكن اضافته مرة اخرى: " + GetPerson.Milnumber.ToString() + " " + GetPerson.Name;
                Handler_Operations.Add_New_Operation_Log(ol_failed);
                return ol_failed;
            }
            frm.Sortingindex = 10000;
            frm.Hasfixedcallerid= 0;
            if (GetPerson.Fixedcallerid != null)
            { 
            if (GetPerson.Fixedcallerid.Trim() != "" && GetPerson.Fixedcallerid != null)
            {
                frm.Hasfixedcallerid= Convert.ToByte(1);
                frm.Callerid= GetPerson.Fixedcallerid.Trim();
            }
            }
            //frm.sunRiseTimeStamp = null;
            //frm.sunSetTimeStamp = null;
            //frm.lastLandTimeStamp = null;
            //frm.incidentID = null;
            frm.Hasdevices = 0;
            //frm.lastAwayTimeStamp = null;
            //frm.lastComeBackTimeStamp = null;
            frm.Patrolpersonstateid = Handler_AhwalMapping.PatrolPersonState_None;

            string InsQry = "";
            if(frm.Patrolroleid == Handler_AhwalMapping.PatrolRole_CaptainAllSectors || frm.Patrolroleid==Handler_AhwalMapping.PatrolRole_CaptainShift)
            {
                InsQry = "insert into AhwalMapping(ahwalid,sectorid,citygroupid,shiftid,patrolroleid,Personid,hasDevices," +
               "patrolPersonStateID,sortingIndex,hasFixedCallerID,callerID) values (" +
               frm.Ahwalid + "," + frm.Sectorid + "," + frm.Citygroupid + "," + frm.Shiftid + "," + frm.Patrolroleid +
                "," + frm.Personid + "," + frm.Hasdevices + "," + frm.Patrolpersonstateid + "," + frm.Sortingindex + "," + frm.Hasfixedcallerid+
               ",'" + frm.Callerid+ "')";
            }
           else if (frm.Patrolroleid == Handler_AhwalMapping.PatrolRole_CaptainSector || frm.Patrolroleid == Handler_AhwalMapping.PatrolRole_SubCaptainSector)
            {
                InsQry = "insert into AhwalMapping(ahwalid,sectorid,citygroupid,shiftid,patrolroleid,Personid,hasDevices," +
               "patrolPersonStateID,sortingIndex,hasFixedCallerID,callerID) values (" +
               frm.Ahwalid + "," + frm.Sectorid + "," + frm.Citygroupid + "," + frm.Shiftid + "," + frm.Patrolroleid +
                "," + frm.Personid + "," + frm.Hasdevices + "," + frm.Patrolpersonstateid + "," + frm.Sortingindex + "," + frm.Hasfixedcallerid+
               ",'" + frm.Callerid+ "')";
            }
            else if (frm.Patrolroleid == Handler_AhwalMapping.PatrolRole_Associate)
            {
                InsQry = "insert into AhwalMapping(ahwalid,sectorid,citygroupid,shiftid,patrolroleid,Personid,hasDevices," +
               "patrolPersonStateID,sortingIndex,hasFixedCallerID,callerID) values (" +
               frm.Ahwalid + "," + frm.Sectorid + "," + frm.Citygroupid + "," + frm.Shiftid + "," + frm.Patrolroleid +
                "," + frm.Personid + "," + frm.Hasdevices + "," + frm.Patrolpersonstateid + "," + frm.Sortingindex + "," + frm.Hasfixedcallerid+
               ",'" + frm.Callerid+ "')";
            }
            else
            {
                InsQry = "insert into AhwalMapping(ahwalid,sectorid,citygroupid,shiftid,patrolroleid,Personid,hasDevices," +
               "patrolPersonStateID,sortingIndex,hasFixedCallerID,callerID) values (" +
               frm.Ahwalid + "," + frm.Sectorid + "," + frm.Citygroupid + "," + frm.Shiftid + "," + frm.Patrolroleid +
                "," + frm.Personid + "," + frm.Hasdevices + "," + frm.Patrolpersonstateid + "," + frm.Sortingindex + "," + frm.Hasfixedcallerid+
               ",'" + frm.Callerid+ "')";
            }

            int ret = DAL.PostGre_ExNonQry(InsQry);
            
                Operationlogs ol = new Operationlogs();
                 ol.Userid = u.Userid;
                ol.Operationid = Handler_Operations.Opeartion_Mapping_AddNew;
                ol.Statusid = Handler_Operations.Opeartion_Status_Success;
                ol.Text = "تم اضافة الفرد: " + GetPerson.Milnumber.ToString() + " " + GetPerson.Name;
                Handler_Operations.Add_New_Operation_Log(ol);
            return ol;

            
        }

        [HttpPost("updateAhwalMapping")]
        public int PostUpDateAhwalMapping([FromBody]Ahwalmapping frm)
        {
            int ret = 0;
            string UpdateQry = "";
            UpdateQry = "update AhwalMapping set ahwalid = " + frm.Ahwalid + ",sectorid=" + frm.Sectorid + ",citygroupid=" + frm.Citygroupid + ",shiftid=" + frm.Shiftid + ",patrolroleid=" + frm.Patrolroleid + ",Personid=" + frm.Personid + " where ahwalmappingid = " + frm.Ahwalmappingid;
            ret = DAL.PostGre_ExNonQry(UpdateQry);
            return ret;
        }

        [HttpDelete("deleteAhwalMapping")]
        public Operationlogs DeleteAhwalMapping([FromQuery]int ahwalMappingID , [FromQuery]int Userid)
        {
            //string ol_label = "";
            Operationlogs ol = new Operationlogs();
            int ret = 0;
            string DelQry = "";
            DelQry = "delete from AhwalMapping where ahwalMappingID = " + ahwalMappingID;
            ret = DAL.PostGre_ExNonQry(DelQry);
            if(ret > 0)
            {
                ol.Userid = Userid;
                ol.Operationid = Handler_Operations.Opeartion_Mapping_Remove;
                ol.Statusid = Handler_Operations.Opeartion_Status_Success;
                ol.Text = "تم حذف الفرد ";  
                return ol;
            }
            ol.Statusid = Handler_Operations.Opeartion_Status_Failed;
            ol.Text = "Failed";
            return ol;
        }


        [HttpGet("cityGroupforAhwal")]
        public List<Citygroups> GetCityGroupForAhwal(int ahwalid)
        {

            string Qry = "SELECT citygroupid, AhwalID, sectorid, shortname,callerprefix,Text,disabled FROM citygroups WHERE AhwalID = " + ahwalid;
            return DAL.PostGre_GetData<Citygroups>(Qry);

        }

        [HttpGet("mappingByID")]
        public Ahwalmapping GetMappingByID(int associateMapID, int Userid)
        {
            string Qry = "SELECT AhwalMapping.Ahwalid, AhwalMapping.Personid, AhwalMapping.Sectorid , AhwalMapping.Citygroupid ,AhwalMapping.Shiftid  FROM AhwalMapping  WHERE AhwalMapping.Ahwalid IN (SELECT AhwalMapping.Ahwalid FROM UsersRolesMap WHERE (Userid = " + Userid + ") and  ahwalmappingid = " + associateMapID; 
          
            List<Ahwalmapping> obj = DAL.PostGre_GetData<Ahwalmapping>(Qry);

            if (obj.Count > 0)
            {
                return obj[0];
            }
            return null;
        }
        
        public Ahwalmapping GetMappingByPersonId(int Personid)
        {
            string Qry = "SELECT AhwalMapping.Ahwalid, AhwalMapping.Personid, AhwalMapping.Sectorid , AhwalMapping.Citygroupid ,AhwalMapping.Shiftid  FROM AhwalMapping  WHERE   Personid = " + Personid;

            List<Ahwalmapping> obj = DAL.PostGre_GetData<Ahwalmapping>(Qry);

            if (obj.Count > 0)
            {
                return obj[0];
            }
            return null;
        }

        [HttpPut("updatePersonState")]
        public Operationlogs updatePersonState([FromQuery]string selmenu,[FromQuery]int ahwalMappingID, [FromQuery]int Userid)
        {
            //string ol_label = "";
            int PatrolPersonStateID = 0;
            if (selmenu == "غياب")
            {
                PatrolPersonStateID = Core.Handler_AhwalMapping.PatrolPersonState_Absent;
            }
            else if (selmenu == "مرضيه")
            {
                PatrolPersonStateID = Core.Handler_AhwalMapping.PatrolPersonState_Sick;
            }
            else if (selmenu == "اجازه")
            {
                PatrolPersonStateID = Core.Handler_AhwalMapping.PatrolPersonState_Off;
            }

            Ahwalmapping person_mapping_exists = GetAhwal(ahwalMappingID);
           
            if (person_mapping_exists == null)
            {
                Operationlogs ol_failed = new Operationlogs();
                ol_failed.Userid = Userid;
                ol_failed.Operationid = Handler_Operations.Opeartion_Mapping_Ahwal_ChangePersonState;
                ol_failed.Statusid = Handler_Operations.Opeartion_Status_Failed;
                ol_failed.Text = "لم يتم العثور على التوزيع";
                Handler_Operations.Add_New_Operation_Log(ol_failed);
                return ol_failed;
            }


            Persons GetPerson  = GetPersonById(person_mapping_exists.Personid);

            if (GetPerson == null)
            {
                Operationlogs ol_failed = new Operationlogs();
                ol_failed.Userid =Userid;
                ol_failed.Operationid = Handler_Operations.Opeartion_Mapping_Ahwal_ChangePersonState;
                ol_failed.Statusid = Handler_Operations.Opeartion_Status_Failed;
                ol_failed.Text = "لم يتم العثور على الفرد: " + person_mapping_exists.Personid; //todo, change it actual person name
                Handler_Operations.Add_New_Operation_Log(ol_failed);
                return ol_failed;
            }

            //last check
            //if he has devices, dont change his state to anything
            if (Convert.ToBoolean(person_mapping_exists.Hasdevices))
            {

                Operationlogs ol_failed = new Operationlogs();
                ol_failed.Userid = Userid;
                ol_failed.Operationid = Handler_Operations.Opeartion_Mapping_Ahwal_ChangePersonState;
                ol_failed.Statusid = Handler_Operations.Opeartion_Status_Failed;
                ol_failed.Text = "هذا المستخدم يملك حاليا اجهزة";
                Handler_Operations.Add_New_Operation_Log(ol_failed);
                return ol_failed;
            }

            Operationlogs ol = new Operationlogs();
            int ret = 0;
            string Qry = "";
            Qry = "update AhwalMapping set PatrolPersonStateID = " + PatrolPersonStateID + " , LastStateChangeTimeStamp = '" + DateTime.Now + "' where AhwalMappingId = " + ahwalMappingID;
            ret = DAL.PostGre_ExNonQry(Qry);
            Qry = "insert into patrolpersonstatelog (Userid,PatrolPersonStateID,TimeStamp,Personid) values(" + Userid + " , " + PatrolPersonStateID + " , '" + DateTime.Now + "' , " + person_mapping_exists.Personid + ")";
            ret = DAL.PostGre_ExNonQry(Qry);
            if (ret > 0)
            {
                ol.Userid = Userid;
                ol.Operationid = Handler_Operations.Opeartion_Mapping_Ahwal_ChangePersonState;
                ol.Statusid = Handler_Operations.Opeartion_Status_Success;
                ol.Text = "احوال تغيير حالة الفرد " + GetPerson.Milnumber + " " + GetPerson.Name ;
                return ol;
            }
            ol.Statusid = Handler_Operations.Opeartion_Status_Failed;
            ol.Text = "Failed";
            return ol;
        }



        #endregion

    }
}