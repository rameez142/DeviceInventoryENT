using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOI.Patrol.DataAccessLayer;
using Npgsql;
using OpenIddict.Validation;
using System;
using System.Collections.Generic;
using System.Data;

namespace MOI.Patrol.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class MaintainenceController : ControllerBase
    {
        public String constr = "server=localhost;Port=5432;User Id=postgres;password=12345;Database=Patrols";
        public DataAccess DAL = new DataAccess();

        [HttpPost("addpatrolcar")]
        public int PostAddPatrolCar([FromBody]Patrolcars frm)
        {
            int ret = 0;
            string Qry = "insert into Patrolcars(AhwalID,platenumber,model,typecode,defective,rental,barcode,vinnumber) values (" + frm.Ahwalid + ",'" + frm.Platenumber + "','" + frm.Model + "','" + frm.Typecode + "'," + frm.Defective + "," + frm.Rental + ",'" + frm.Barcode + "','" + frm.Vinnumber + "')";
            ret = DAL.PostGre_ExNonQry(Qry);
            return ret;
        }

        [HttpPost("updatepatrolcar")]
        public int PostUpdatePatrolCar([FromBody] Patrolcars frm)
        {
            int ret = 0;
            string Qry = "update Patrolcars set AhwalID = " + frm.Ahwalid + ",platenumber = '" + frm.Platenumber + "',model = '" + frm.Model + "',typecode='" + frm.Typecode + "',defective = " + frm.Defective + ",rental = " + frm.Rental + ",barcode = '" + frm.Barcode + "',vinnumber='" + frm.Vinnumber + "' where patrolid=" + frm.Patrolid;
            ret = DAL.PostGre_ExNonQry(Qry);
            return ret;
        }


        [HttpPost("delpatrolcar")]
        public int PostDeletePatrolCar([FromBody] Patrolcars frm)
        {
            int ret = 0;
            string Qry = "update Patrolcars set delflag='1' where patrolid=" + frm.Patrolid;
            ret = DAL.PostGre_ExNonQry(Qry);
            return ret;
        }


        [Authorize("ViewPatrolCarsRole")]
        [HttpPost("patrolcarslist")]
        public List<Patrolcars> PostPatrolCarsList([FromBody] int ahwalid)
        {

            string subqry = "";
            subqry = " and d.Ahwalid in (select ahwalid from UsersRolesMap where UserID=6)";
            if (ahwalid != -1)
            {
                subqry = subqry + " and d.Ahwalid = " + ahwalid;
            }
            String Qry = "select (select a.name from ahwal a where  a.Ahwalid = d.Ahwalid) ahwalname,d.Ahwalid, d.Patrolid,d.Platenumber,d.Model,(select codedesc from codemaster where code = typecode)  as type,typecode,d.Defective,d.Rental,d.Barcode,vinnumber from Patrolcars d where d.delflag is null  " + subqry;
            List<Patrolcars> ptc = DAL.PostGre_GetData<Patrolcars>(Qry);
            return ptc;

        }



        [HttpGet("patrolcarsinventory")]
        public DataTable PostPatrolCarsInventoryList(int ahwalid, int userid)
        {

            string subqry = "";

            if (ahwalid != -1)
            {
                subqry = subqry + " and Ahwal.Ahwalid = " + ahwalid;
            }

            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            DataTable dt = new DataTable();
            string Qry = "SELECT        patrolcheckinoutid, CheckInOutStates.Name AS StateName, Ahwal.Ahwalid, Ahwal.Name AS AhwalName, Patrolcars.Platenumber, Patrolcars.Model,'' as Type, Persons.MilNumber, ";
            Qry = Qry + " Ranks.Name AS PersonRank, Persons.Name AS PersonName, patrolCheckInOut.timestamp, CheckInOutStates.CheckInOutStateID";

            Qry = Qry + "  FROM Ahwal INNER JOIN";

            Qry = Qry + " Patrolcars  ON Ahwal.Ahwalid = Patrolcars.Ahwalid INNER JOIN";

            Qry = Qry + " patrolCheckInOut ON Patrolcars.Patrolid = patrolCheckInOut.Patrolid INNER JOIN";

            Qry = Qry + " CheckInOutStates ON patrolCheckInOut.CheckInOutStateID = CheckInOutStates.CheckInOutStateID INNER JOIN";

            Qry = Qry + " Persons ON Ahwal.Ahwalid = Persons.Ahwalid AND patrolCheckInOut.PersonID = Persons.PersonID INNER JOIN";

            Qry = Qry + " Ranks ON Persons.RankID = Ranks.RankID";
            Qry = Qry + " where Ahwal.Ahwalid IN (SELECT AhwalID FROM UsersRolesMap WHERE UserID = " + userid + " ) ";
            Qry = Qry + subqry;
            Qry = Qry + "  ORDER BY patrolCheckInOut.timestamp";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(Qry, cont);
            dt.Columns.Add("patrolcheckinoutid");
            dt.Columns.Add("statename");
            dt.Columns.Add("ahwalid");
            dt.Columns.Add("ahwalname");
            dt.Columns.Add("platenumber");
            dt.Columns.Add("model");

            dt.Columns.Add("type");
            dt.Columns.Add("milnumber");
            dt.Columns.Add("personrank");
            dt.Columns.Add("personname");
            dt.Columns.Add("timestamp");
            dt.Columns.Add("checkinoutstateid");
            da.Fill(dt);
            cont.Close();
            cont.Dispose();


            return dt;
        }




        [HttpPost("organizationlist")]
        public DataTable PostOrganizationList([FromBody] int userid)
        {
            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            DataTable dt = new DataTable();
            String Qry = "select '-1'  as value,'' as text  union all SELECT ahwalid as value, name as text FROM Ahwal where ahwalid in (select ahwalid from usersrolesmap where userid = " + userid + ")";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(Qry, cont);
            da.Fill(dt);
            cont.Close();
            cont.Dispose();

            return dt;
        }


        [HttpPost("checkuser")]
        public DataTable PostCheckUser()
        {
            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            DataTable dt = new DataTable();
            String Qry = "SELECT ahwalid as value, name as text FROM Ahwal ";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(Qry, cont);
            da.Fill(dt);
            cont.Close();
            cont.Dispose();

            return dt;
        }

        [HttpPost("patrolcartypes")]
        public DataTable Postdevicetyplist()
        {
            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            DataTable dt = new DataTable();
            String Qry = "select 'xx'  as value,'' as text  union all SELECT code as value, codedesc as text FROM codemaster";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(Qry, cont);
            da.Fill(dt);

            cont.Close();
            cont.Dispose();

            return dt;
        }


        [HttpGet("checkinpatrolcarslist")]
        public List<Patrolcars> GetCheckinPatrolCarList([FromQuery] int ahwalid, [FromQuery] int userid)
        {

            string subqry = "";
            subqry = " and d.Ahwalid in (select ahwalid from UsersRolesMap where UserID= " + userid  + " )";
            if (ahwalid != -1)
            {
                subqry = subqry + " and d.Ahwalid = " + ahwalid;
            }
            String Qry = "select (select a.name from ahwal a where  a.Ahwalid = d.Ahwalid) ahwalname,d.Ahwalid, d.Patrolid,d.Platenumber,d.Model,(select codedesc from codemaster where code = typecode)  as type,typecode,d.Defective,d.Rental,d.Barcode,vinnumber from Patrolcars d where d.delflag is null  " + subqry;
            List<Patrolcars> ptc = DAL.PostGre_GetData<Patrolcars>(Qry);
            return ptc;

        }
        

        /*Hand Helds*/
        #region Hand Helds
        [HttpPost("addhandheld")]
        public int PostAddHandhelds([FromBody]Handhelds frm)
        {
            int ret = 0;
            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = cont;
            cmd.CommandText = "insert into Handhelds(AhwalID,serial,defective,barcode) values (" + frm.Ahwalid + ",'" + frm.Serial + "'," + frm.Defective + ",'" + frm.Barcode + "')";
            ret = cmd.ExecuteNonQuery();
            cont.Close();
            cont.Dispose();


            return ret;
        }

        [HttpPost("updatehandheld")]
        public int PostUpdateHandhelds([FromBody] Handhelds frm)
        {
            int ret = 0;
            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = cont;
            cmd.CommandText = "update Handhelds set AhwalID = " + frm.Ahwalid + ",serial = '" + frm.Serial + "',defective = " + frm.Defective + ",barcode = '" + frm.Barcode + "' where handheldid=" + frm.Handheldid;
            ret = cmd.ExecuteNonQuery();
            cont.Close();
            cont.Dispose();


            return ret;
        }


        [HttpPost("delhandheld")]
        public int PostDeletehandheld([FromBody] Handhelds frm)
        {
            int ret = 0;
            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = cont;
            cmd.CommandText = "delete from Handhelds  where handheldid=" + frm.Handheldid;
            ret = cmd.ExecuteNonQuery();
            cont.Close();
            cont.Dispose();
            return ret;
        }




        [HttpGet("handheldlist")]
        public DataTable GetHandHeldList(int ahwalid, int userid)
        {
            string subqry = "";
            if (ahwalid != -1)
            {
                subqry = " and d.Ahwalid = " + ahwalid;
            }

            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            DataTable dt = new DataTable();
            //            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.deviceid,d.DeviceNumber,d.Model,t.name as type,d.Defective,d.Rental,d.Barcode,a.Name from Devices d INNER JOIN Ahwal a ON a.Ahwalid = d.Ahwalid inner join devicetypes t on t.devicetypeid = d.devicetypeid ", cont);
            //NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.deviceid,d.DeviceNumber,d.Model,(select dt.name from devicetypes dt where dt.devicetypeid = d.devicetypeid)  as type,d.Defective,d.Rental,d.Barcode,'jjjj' as Name from Devices d", cont);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.Handheldid,d.Serial,d.Defective,d.Barcode,d.Ahwalid,(select a.name from ahwal a where a.Ahwalid = d.Ahwalid ) ahwalname from Handhelds d where d.Serial is not null AND AhwalID IN (SELECT AhwalID FROM UsersRolesMap WHERE UserID = " + userid + " ) ", cont);

            // NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.deviceid,d.DeviceNumber,d.Model,'1'  as type,d.Defective,d.Rental,d.Barcode,'jjjj' as Name from Devices d", cont);
            da.Fill(dt);
            cont.Close();
            cont.Dispose();


            return dt;
        }


        

        [HttpGet("checkinhandheldslist")]
        public List<Handhelds> GetCheckinHandHeldList([FromQuery] int ahwalid, [FromQuery] int userid)
        {

            string subqry = "";
            subqry = " and d.Ahwalid in (select ahwalid from UsersRolesMap where UserID= " + userid + " )";
            if (ahwalid != -1)
            {
                subqry = subqry + " and d.Ahwalid = " + ahwalid;
            }
            String Qry = "select d.Handheldid,d.Serial,d.Defective,d.Barcode,d.Ahwalid,(select a.name from ahwal a where a.Ahwalid = d.Ahwalid ) ahwalname from Handhelds d where d.Serial is not null AND AhwalID IN(SELECT AhwalID FROM UsersRolesMap WHERE UserID = " + userid + ") ";
            List<Handhelds> ptc = DAL.PostGre_GetData<Handhelds>(Qry);
            return ptc;

        }


        #endregion

        /*Accessory*/
        #region Accessory
        //[HttpPost("addaccessories")]
        //public int PostAddaccessories([FromBody]Devices frm)
        //{
        //    int ret = 0;
        //    NpgsqlConnection cont = new NpgsqlConnection();
        //    cont.ConnectionString = constr;
        //    cont.Open();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    cmd.Connection = cont;
        //    cmd.CommandText = "insert into devices(AhwalID,devicenumber,model,devicetypeid,defective,rental,barcode) values (" + frm.Ahwalid + ",'" + frm.devicenumber + "'," + frm.Model + "," + frm.devicetypeid + "," + frm.Defective + "," + frm.Rental + ",'" + frm.Barcode + "')";
        //    ret = cmd.ExecuteNonQuery();
        //    cont.Close();
        //    cont.Dispose();


        //    return ret;
        //}

        //[HttpPost("updateaccessories")]
        //public int PostUpdateaccessories([FromBody] Devices frm)
        //{
        //    int ret = 0;
        //    NpgsqlConnection cont = new NpgsqlConnection();
        //    cont.ConnectionString = constr;
        //    cont.Open();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    cmd.Connection = cont;
        //    cmd.CommandText = "update devices set AhwalID = " + frm.Ahwalid + ",devicenumber = '" + frm.devicenumber + "',model = '" + frm.Model + "',devicetypeid='" + frm.devicetypeid + "',defective = " + frm.Defective + ",rental = " + frm.Rental + ",barcode = '" + frm.Barcode + "' where deviceid=" + frm.deviceid;
        //    ret = cmd.ExecuteNonQuery();
        //    cont.Close();
        //    cont.Dispose();


        //    return ret;
        //}


        //[HttpPost("delaccessorie")]
        //public int PostDeleteaccessorie([FromBody] Devices frm)
        //{
        //    int ret = 0;
        //    NpgsqlConnection cont = new NpgsqlConnection();
        //    cont.ConnectionString = constr;
        //    cont.Open();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    cmd.Connection = cont;
        //    cmd.CommandText = "delete from devices  where deviceid=" + frm.deviceid;
        //    ret = cmd.ExecuteNonQuery();
        //    cont.Close();
        //    cont.Dispose();
        //    return ret;
        //}




        //[HttpPost("accessorielist")]
        //public DataTable PostaccessorieList()
        //{


        //    NpgsqlConnection cont = new NpgsqlConnection();
        //    cont.ConnectionString = constr;
        //    cont.Open();
        //    DataTable dt = new DataTable();
        //    //            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.deviceid,d.DeviceNumber,d.Model,t.name as type,d.Defective,d.Rental,d.Barcode,a.Name from Devices d INNER JOIN Ahwal a ON a.Ahwalid = d.Ahwalid inner join devicetypes t on t.devicetypeid = d.devicetypeid ", cont);
        //    NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.deviceid,d.DeviceNumber,d.Model,(select dt.name from devicetypes dt where dt.devicetypeid = d.devicetypeid)  as type,d.Defective,d.Rental,d.Barcode,'jjjj' as Name from Devices d", cont);
        //    // NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.deviceid,d.DeviceNumber,d.Model,'1'  as type,d.Defective,d.Rental,d.Barcode,'jjjj' as Name from Devices d", cont);
        //    da.Fill(dt);
        //    cont.Close();
        //    cont.Dispose();


        //    return dt;
        //}


        #endregion

        /*Persons*/
        #region Persons
        //[HttpPost("addpersons")]
        //public int PostAddpersons([FromBody]Devices frm)
        //{
        //    int ret = 0;
        //    string InsQry = "";
        //    //we have to check first that this person doesn't exists before in mapping
        //    InsQry = "insert into devices(AhwalID,devicenumber,model,devicetypeid,defective,rental,barcode) values (" + frm.Ahwalid + ",'" + frm.devicenumber + "'," + frm.Model + "," + frm.devicetypeid + "," + frm.Defective + "," + frm.Rental + ",'" + frm.Barcode + "')";
        //    ret = DAL.PostGre_ExNonQry(InsQry);
        //    return ret;
        //}



        //[HttpPost("updatepersons")]
        //public int PostUpdatepersons([FromBody] Devices frm)
        //{
        //    int ret = 0;
        //    NpgsqlConnection cont = new NpgsqlConnection();
        //    cont.ConnectionString = constr;
        //    cont.Open();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    cmd.Connection = cont;
        //    cmd.CommandText = "update devices set AhwalID = " + frm.Ahwalid + ",devicenumber = '" + frm.devicenumber + "',model = '" + frm.Model + "',devicetypeid='" + frm.devicetypeid + "',defective = " + frm.Defective + ",rental = " + frm.Rental + ",barcode = '" + frm.Barcode + "' where deviceid=" + frm.deviceid;
        //    ret = cmd.ExecuteNonQuery();
        //    cont.Close();
        //    cont.Dispose();


        //    return ret;
        //}


        //[HttpPost("delperson")]
        //public int PostDeleteperson([FromBody] Devices frm)
        //{
        //    int ret = 0;
        //    NpgsqlConnection cont = new NpgsqlConnection();
        //    cont.ConnectionString = constr;
        //    cont.Open();
        //    NpgsqlCommand cmd = new NpgsqlCommand();
        //    cmd.Connection = cont;
        //    cmd.CommandText = "delete from devices  where deviceid=" + frm.deviceid;
        //    ret = cmd.ExecuteNonQuery();
        //    cont.Close();
        //    cont.Dispose();
        //    return ret;
        //}




        //[HttpPost("personlist")]
        //public DataTable PostpersonList()
        //{


        //    NpgsqlConnection cont = new NpgsqlConnection();
        //    cont.ConnectionString = constr;
        //    cont.Open();
        //    DataTable dt = new DataTable();
        //    //            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.deviceid,d.DeviceNumber,d.Model,t.name as type,d.Defective,d.Rental,d.Barcode,a.Name from Devices d INNER JOIN Ahwal a ON a.Ahwalid = d.Ahwalid inner join devicetypes t on t.devicetypeid = d.devicetypeid ", cont);
        //    NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.deviceid,d.DeviceNumber,d.Model,(select dt.name from devicetypes dt where dt.devicetypeid = d.devicetypeid)  as type,d.Defective,d.Rental,d.Barcode,'jjjj' as Name from Devices d", cont);
        //    // NpgsqlDataAdapter da = new NpgsqlDataAdapter("select d.deviceid,d.DeviceNumber,d.Model,'1'  as type,d.Defective,d.Rental,d.Barcode,'jjjj' as Name from Devices d", cont);
        //    da.Fill(dt);
        //    cont.Close();
        //    cont.Dispose();


        //    return dt;
        //}


        #endregion

        #region Hand Held Invenory
        [HttpGet("handheldinventory")]
        public DataTable PostHandHeldInventoryList(int ahwalid, int userid)
        {
            string subqry = "";
            if (ahwalid != -1)
            {
                subqry = " and d.Ahwalid = " + ahwalid;
            }

            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            DataTable dt = new DataTable();
            string Qry = "SELECT        HandHeldsCheckInOut.HandHeldCheckInOutID,HandHeldsCheckInOut.TimeStamp, CheckInOutStates.CheckInOutStateID,CheckInOutStates.Name AS StateName, Handhelds.Ahwalid, Handhelds.Serial, Ranks.Name as PersonRank, Persons.MilNumber, Persons.Name AS PersonName ";

            Qry = Qry + " ,Ahwal.Name as ahwalname FROM Ahwal INNER JOIN";

            Qry = Qry + " Handhelds  ON Ahwal.Ahwalid = Handhelds.Ahwalid INNER JOIN";

            Qry = Qry + " HandHeldsCheckInOut ON Handhelds.Handheldid = HandHeldsCheckInOut.Handheldid INNER JOIN";

            Qry = Qry + " CheckInOutStates ON HandHeldsCheckInOut.CheckInOutStateID = CheckInOutStates.CheckInOutStateID INNER JOIN";
            Qry = Qry + " Persons ON Ahwal.Ahwalid = Persons.Ahwalid AND HandHeldsCheckInOut.PersonID = Persons.PersonID INNER JOIN";
            Qry = Qry + " Ranks ON Persons.RankID = Ranks.RankID where  Ahwal.Ahwalid IN (SELECT AhwalID FROM UsersRolesMap WHERE UserID = " + userid + " ) ";

            Qry = Qry + "  ORDER BY HandHeldsCheckInOut.timestamp";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(Qry, cont);
            dt.Columns.Add("patrolcheckinoutid");
            dt.Columns.Add("statename");
            dt.Columns.Add("ahwalid");
            dt.Columns.Add("ahwalname");
            dt.Columns.Add("platenumber");
            dt.Columns.Add("model");
            dt.Columns.Add("milnumber");
            dt.Columns.Add("personrank");
            dt.Columns.Add("personname");
            dt.Columns.Add("timestamp");
            dt.Columns.Add("checkinoutstateid");
            da.Fill(dt);
            cont.Close();
            cont.Dispose();



            return dt;
        }

        #endregion

        #region Accessory Inventory
        [HttpPost("accessoryinventory")]
        public DataTable PostAccessoryInventoryList()
        {


            NpgsqlConnection cont = new NpgsqlConnection();
            cont.ConnectionString = constr;
            cont.Open();
            DataTable dt = new DataTable();
            string Qry = "SELECT        patrolcheckinoutid, CheckInOutStates.Name AS StateName, Ahwal.Ahwalid, Ahwal.Name AS AhwalName, Patrolcars.Platenumber, Patrolcars.Model,'' as Type, Persons.MilNumber, ";
            Qry = Qry + " Ranks.Name AS PersonRank, Persons.Name AS PersonName, patrolCheckInOut.timestamp, CheckInOutStates.CheckInOutStateID";

            Qry = Qry + "  FROM Ahwal INNER JOIN";

            Qry = Qry + " Patrolcars  ON Ahwal.Ahwalid = Patrolcars.Ahwalid INNER JOIN";

            Qry = Qry + " patrolCheckInOut ON Patrolcars.Patrolid = patrolCheckInOut.Patrolid INNER JOIN";

            Qry = Qry + " CheckInOutStates ON patrolCheckInOut.CheckInOutStateID = CheckInOutStates.CheckInOutStateID INNER JOIN";

            Qry = Qry + " Persons ON Ahwal.Ahwalid = Persons.Ahwalid AND patrolCheckInOut.PersonID = Persons.PersonID INNER JOIN";

            Qry = Qry + " Ranks ON Persons.RankID = Ranks.RankID";
            Qry = Qry + "  ORDER BY patrolCheckInOut.timestamp";

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(Qry, cont);
            dt.Columns.Add("patrolcheckinoutid");
            dt.Columns.Add("statename");
            dt.Columns.Add("ahwalid");
            dt.Columns.Add("ahwalname");
            dt.Columns.Add("platenumber");
            dt.Columns.Add("model");

            dt.Columns.Add("type");
            dt.Columns.Add("milnumber");
            dt.Columns.Add("personrank");
            dt.Columns.Add("personname");
            dt.Columns.Add("timestamp");
            dt.Columns.Add("checkinoutstateid");
            da.Fill(dt);
            cont.Close();
            cont.Dispose();



            return dt;
        }

        #endregion






    }


}
