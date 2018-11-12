import { ElementRef,Component, OnInit ,ViewChild} from '@angular/core';
import { CommonService } from '../../../services/common.service';
import { DxDataGridComponent, DxSelectBoxComponent } from 'devextreme-angular'
import notify from 'devextreme/ui/notify';
import { ModalService } from '../../../services/modalservice';
import { handler_ahwalMapping } from '../../../../environments/handler_ahwalMapping';
import {ahwalmapping} from '../../../models/ahwalmapping';
import {citygroups} from '../../../models/citygroups';
import {persons} from '../../../models/persons';
import { patrolcars } from '../../../models/patrolcars';
import { handhelds } from '../../../models/handhelds';
import { associates } from '../../../models/associates';
import { sectors } from '../../../models/sectors';
import { shifts } from '../../../models/shifts';
import { patrolroles } from '../../../models/patrolroles';
import { user } from '../../../models/user';
import { operationLog } from '../../../models/operationLog';

import { handler_operations } from '../../../../environments/handler_operations';
import { HandheldinventoryComponent } from '../../maintainence/inventory/handheldinventory/handheldinventory.component';

@Component({
  selector: 'app-dispatch',
  templateUrl: './dispatch.component.html',
  styleUrls: ['./dispatch.component.css']
})



export class DispatchComponent implements OnInit {

  @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;



  loadingVisible:boolean = false;
  selahwalid:number = -1;

ahwalMapping_CheckInOut_StatusLabel:string;
responsibilitysrc:patrolroles[];
shiftssrc:shifts[];
sectorssrc:sectors[];
citysrc:citygroups[];
associatesrc:associates[];
sectorid:number;
personsrc:persons[];
shiftvisibile:boolean=false;
sectorvisibile:boolean=false;
cityvisibile:boolean=false;
associatevisibile:boolean=false;
userid:number = null;
selectedRole:string = null;
selectedShift:string = null;
ahwalMapping_Add_status_label:string ='';
selectPerson_Mno:string =null;
ahwalMappingAddMethod:string ='';
selahwalmappingid:number=null;
selectedSector:string=null;
selectedAssociateMapId:string=null;
selectedCity:string=null;
checkInOutPopupVisible:any=false;
personname:string='';
associatePersonMno:number = null;
menuOpen:boolean=false;
userobj:user = new user();
dataSource: any;
styleExp:string='none';
AhwalMapping_CheckInOut_ID:any;
AhwalMapping_CheckInOut_Method:any;
selCheckInOutPersonMno: number =null;
selCheckInOutPatrolPltNo:number = null;
selCheckInOutHHeldSerialNo:number = null;

patrolCarsrc:patrolcars[];
selRowIndex:number;
  public options = {
    spinable: true,
    buttonWidth: 40,
    defaultOpen:true
};
handHeldsrc:handhelds[];
public wings = [{
  'title': 'Add Person',
  'color': '#ea2a29',
  'icon': {'name': ''}
}, {
  'title': 'غياب',
  'color': '#f16729',
  'icon': {'name': ''}
}, {
  'title': 'مرضيه',
  'color': '#f89322',
  'icon': {'name': ''}
}, {
  'title': 'اجازه',
  'color': '#ffcf14',
  'icon': {'name': ''}
},
 {
  'title': 'حذف',
  'color': '#ffcf20',
  'icon': {'name': ''}
}
, {
  'title': 'CheckIn/Out',
  'color': '#ffcf16',
  'icon': {'name': ''}
}
];

public gutter = {
top: 400,
left:600
};

public startAngles = {
topLeft: -20
};


  constructor(private svc:CommonService, private modalService: ModalService) {
      this.userid = parseInt(window.localStorage.getItem('UserID'),10);
      this.userobj.userID = this.userid;
    this.showLoadPanel();
   }

  onShown() {
    setTimeout(() => {
        this.loadingVisible = false;
    }, 3000);
  }


  showLoadPanel() {
    this.loadingVisible = true;
  }





  ngOnInit() {

    this.loadDataSources();
    this.loadData();
  }

  roleSelection(e)
  {
    console.log(e);
this.selectedRole = (e.value);

if(e.value !== null)
{
if(parseInt(e.value, 10) === handler_ahwalMapping.PatrolRole_CaptainAllSectors ||
 parseInt(e.value,10) === handler_ahwalMapping.PatrolRole_CaptainShift)
{
  this.shiftvisibile = true;
  this.sectorvisibile = false;
  this.cityvisibile = false;
 // this.searchInput.nativeElement.visible = false;
  this.associatevisibile = false;
}
else if(parseInt(e.value,10) === handler_ahwalMapping.PatrolRole_CaptainSector ||
parseInt(e.value,10) === handler_ahwalMapping.PatrolRole_SubCaptainSector)
{
  this.shiftvisibile = true;
  this.sectorvisibile = true;
  this.cityvisibile = false;
  //this.searchInput.nativeElement.visible = false;
  this.associatevisibile = false;
}
else if( parseInt(e.value , 10) === handler_ahwalMapping.PatrolRole_Associate)
{
  this.shiftvisibile = false;
  this.sectorvisibile = false;
  this.cityvisibile = false;
 // this.searchInput.nativeElement.visible = false;
  this.associatevisibile = true;
}
else if (parseInt(e.value , 10) != -1 && parseInt(e.value ,10) != null)
{
    this.shiftvisibile = true;
    this.sectorvisibile = true;
    this.cityvisibile = true;
   // this.searchInput.nativeElement.visible = true;
    this.associatevisibile = false;
}
}

  }

   loadDataSources()
  {

     this.svc.GetShiftsList().toPromise().then(resp =>
        {
           // console.log(resp);
           this.shiftssrc = <shifts[]>resp;
      },
        error => {
        });

         this.svc.GetResponsibiltyList().toPromise().then(resp =>
        {
            console.log(resp);
               this.responsibilitysrc = <patrolroles[]>resp;
               console.log(this.responsibilitysrc);
        },
        error => {
        });

         this.svc.GetSectorsList(this.userid).toPromise().then(resp =>
        {
            console.log(resp);
                   this.sectorssrc = <sectors[]>resp;
                   console.log(this.sectorssrc);
         });



          this.svc.GetAssociateList(this.userid).toPromise().then(resp =>
         {
         this.associatesrc = <associates[]>resp;
          });

           this.svc.GetPersonList(this.userid).toPromise().then(resp =>
           {

            this.personsrc = <persons[]> resp;
            });

            this.svc.GetCheckinPatrolCarList(this.selahwalid,this.userid).toPromise().then(resp =>
                {
     
                 this.patrolCarsrc = <patrolcars[]> resp;
                 });

                 this.svc.GetCheckinHandHeldList(this.selahwalid,this.userid).toPromise().then(resp =>
                    {
         
                     this.handHeldsrc = <handhelds[]> resp;
                     });

  }



sectorSelection(e)
{

   if(e.value !== null)
   {
    this.selectedSector = e.value;
    this.svc.GetCityList(this.userid,parseInt(this.selectedSector , 10)).subscribe(resp =>
        {
                   this.citysrc = <citygroups[]>resp;
         });
   }
    else
    {
        this.citysrc = [];
    }
}


person_displayExpr(item){
   // console.log(item);
   if(item !== undefined)
   { return  item.name ;
   }
}


associateExpr(item){
    if ( item !== undefined )
    { return  item.name ;
    }

}

checkPatrolExp(item)
{
    if ( item !== undefined )
    { return  item.platenumber ;
    }
}

loadData()
{
  this.svc.GetDispatchList().subscribe(resp =>
    {

       this.dataSource = resp;
     // console.log('resp' + resp);
      this.dataGrid.dataSource = this.dataSource;
      this.dataGrid.instance.refresh();

  },
    error => {

    });
}

onToolbarPreparing(e) {
    let strt :any=[];
strt =JSON.parse(window.localStorage.getItem('Orgs'));
  e.toolbarOptions.items.unshift({
      location: 'before',
      template: 'الأحوال'
  }, {
          location: 'before',
          widget: 'dxSelectBox',
          options: {
              width: 200,
              items: strt,
              displayExpr: 'text',
              valueExpr: 'value',
              value: '1',
              onValueChanged: this.ahwalChanged.bind(this)
          }
      },{
        location: 'before',
        template: 'الشفت'
    },{
        location: 'before',
        widget: 'dxSelectBox',
        options: {
            width: 200,
            items: [{
                value: '1',
                text: 'صباح'
            }, {
                value: '2',
                text: 'عصر'
            },
            {
                value: '3',
                text: ''
            }
        ],
            displayExpr: 'text',
            valueExpr: 'value',
            value: '1'
        }
    }, {
        location: 'before',
        widget: 'dxButton',
        options: {
            icon: 'glyphicon glyphicon-user',
            onClick: this.showInfo.bind(this)
        }
    }
    , {
          location: 'after',
          widget: 'dxButton',
          options: {
              icon: 'refresh',
              onClick: this.refreshDataGrid.bind(this)
          }
      });
}

ahwalChanged(e) {
    this.selahwalid = e.value;
    this.loadData();

}

groupChanged(e) {
    if (parseInt(e.value , 10 ) === 3)
    {
        this.loadData();
        this.refreshDataGrid();
    }

}

onRowPrepared(e)
{
    //if(e.RowType)

    if(e.rowType ==='data')
    {
     //  console.log(e);

        if(e.key.patrolpersonstateid === 20 || e.key.patrolpersonstateid === 30 ||
          e.key.patrolpersonstateid === 40 || e.key.patrolpersonstateid === 74)
        {
            e.rowElement.bgColor='#a0d89e';

        }
        if(e.key.patrolpersonstateid === 70 )
        {
            e.rowElement.bgColor='#bfbeaa';

        }
        if(e.key.patrolpersonstateid === 60 )
        {
            e.rowElement.bgColor='#edeb9e';

        }
        if(e.key.patrolpersonstateid === 100 || e.key.patrolpersonstateid === 80 || e.key.patrolpersonstateid === 90 )
        {
            e.rowElement.bgColor='#ea88c8';

        }
        if(e.key.patrolpersonstateid === 72  )
        {
            e.rowElement.bgColor='#ea88c8';

        }
        if(e.key.patrolpersonstateid === 70  )
        {
            e.rowElement.bgColor='sandybrown';

        }

    }
}

WingSelected(e)
{
  console.log(e);
if(e === false)
{
  this.styleExp = 'none';
}

}
WingSelected2(e)
{

    if(e.title ==='Add Person')
  {
    this.popupVisible = true;
  }
  else if(e.title ==='حذف')
  {
    
    this.deleteMapping();
  }
  else if(e.title ==='غياب' ||e.title ==='مرضيه'  || e.title ==='اجازه' )
  {
      
    this.updatePersonState(e.title);
  }
  else if(e.title ==='CheckIn/Out'  )
  {
     
    this.CallDblClick();
  }
}

updatePersonState(selmenu:string)
{
    if(this.selahwalmappingid !== null)
    {
        
      this.svc.updatePersonState(selmenu,this.selahwalmappingid,this.userid).toPromise().then(resp =>
      {
        let olog:operationLog = new operationLog();
        olog= <operationLog>resp;
        notify( olog.text, 'success', 600);
        this.loadData();
  
    });
  
    }
}
 deleteMapping() {
console.log(this.selahwalmappingid);
  if(this.selahwalmappingid !== null)
  {
    this.svc.DeleteAhwalMapping(this.selahwalmappingid,this.userid).toPromise().then(resp =>
    {
      let olog:operationLog = new operationLog();
      olog= <operationLog>resp;
      notify( olog.text, 'success', 600);
      this.loadData();

  });

  }
}
onContextMenuprepare(e) {
  //this.menuOpen = true;
  console.log(e);
  this.selahwalmappingid = e.row.key.ahwalmappingid;
  this.selCheckInOutPersonMno = e.row.key.milnumber;
  //console.log(this.selahwalmappingid);
  this.options.defaultOpen = true;
  this.styleExp = 'inline';
  if (e.row.rowType === 'data') {
    e.items = [{text:''}];
  }

  e.cancel = true;

    /* if (e.row.rowType === 'data') {
    e.items = [{
      text: 'غياب',
      value:e.row.rowIndex

  },
  {
      text: 'مرضيه',
      value:e.row.rowIndex
  }
  ,
  {
      text: 'اجازه',
      value:e.row.rowIndex
  }
];

  } */
}

refreshDataGrid() {
  this.dataGrid.instance.refresh();
}

popupVisible:any = false;
showInfo() {
    this.clearpersonpopupvalues();
    this.ahwalMappingAddMethod ='ADD';
    this.popupVisible = true;
   //this.modalService.open('custom-modal-2');
}

mappopupVisible:any = false;

showmapInfo() {
  this.modalService.open('custom-modal-1');
}

citySelection(e)
{
this.selectedCity = e.value;
}

async AhwalMapping_Add_SubmitButton_Click(e)
{

   if(this.selectedRole === null)
   {
    this.ahwalMapping_Add_status_label  = 'يرجى اختيار المسؤولية';
    return;
   }

  if(this.selectPerson_Mno === null){
    this.ahwalMapping_Add_status_label  = 'يرجى اختيار الفرد';
    return;
}

let personobj:persons = null;
  //let myAdd = async function() {
     await this.svc.GetPersonForUserForRole(parseInt(this.selectPerson_Mno , 10),
   this.userid).toPromise().then(resp =>
    {
        if (resp !== [])
        {
            personobj = <persons>resp;
            console.log(personobj);
        }
        console.log(personobj);
  });
//};

console.log(personobj);

  if (personobj === null)
  {
      console.log(personobj);
      this.ahwalMapping_Add_status_label = 'لم يتم العثور على الفرد';
      return;
  }


console.log(personobj);
let ahwalmappingobj:ahwalmapping = new ahwalmapping();
if(parseInt(this.selectedRole , 10) === handler_ahwalMapping.PatrolRole_CaptainAllSectors ||
 parseInt(this.selectedRole, 10 ) === handler_ahwalMapping.PatrolRole_CaptainShift)
{

    if(this.selectedShift === null)
    {
        this.ahwalMapping_Add_status_label  = 'يرجى اختيار الشفت';
        return;
    }

    ahwalmappingobj.ahwalID = personobj.ahwalID;
    ahwalmappingobj.personID = personobj.personID;
    ahwalmappingobj.sectorID = handler_ahwalMapping.Sector_Public;
    let citygroupsobj:citygroups[];
    await this.svc.GetCityGroupForAhwal(personobj.ahwalID).toPromise().then (resp =>
        {

            if (resp !== [])
            {
                citygroupsobj = <citygroups[]>(resp);
            }

      },
        error => {
        });
        ahwalmappingobj.cityGroupID = citygroupsobj[0].cityGroupID;
        ahwalmappingobj.shiftID = parseInt(this.selectedShift , 10);
        ahwalmappingobj.patrolRoleID = parseInt(this.selectedRole , 10);
        if(this.ahwalMappingAddMethod === 'UPDATE') {
            ahwalmappingobj.ahwalMappingID = this.selahwalmappingid;
            this.svc.UpDateAhwalMapping(ahwalmappingobj).subscribe(resp =>
                {
                    this.clearpersonpopupvalues();
                    this.ahwalMapping_Add_status_label = resp;
                 this.loadData();
              });
        }
        else {
            this.svc.AddAhwalMapping(ahwalmappingobj,this.userobj).subscribe(resp =>
                {
                    this.clearpersonpopupvalues();
                  let olog:operationLog = new operationLog();
                 olog= <operationLog>resp;
                    this.ahwalMapping_Add_status_label = olog.text;
                 this.loadData();
              });
        }
}
else if (parseInt(this.selectedRole,10) === handler_ahwalMapping.PatrolRole_CaptainSector ||
parseInt(this.selectedRole,10) === handler_ahwalMapping.PatrolRole_SubCaptainSector)
{
    if(this.selectedShift === null)
    {
        this.ahwalMapping_Add_status_label  = 'يرجى اختيار الشفت';
        return;
    }

    if(this.selectedSector === null)
    {
        this.ahwalMapping_Add_status_label  = 'يرجى اختيار القطاع';
        return;
    }
    ahwalmappingobj.ahwalID = personobj.ahwalID;
    ahwalmappingobj.personID = personobj.personID;
    ahwalmappingobj.sectorID = parseInt(this.selectedSector,10);
    let citygroupsobj:citygroups[];
    await this.svc.GetCityGroupForAhwal(personobj.ahwalID,ahwalmappingobj.sectorID).toPromise().then(resp =>
        {

            if (resp !== [])
            {
                citygroupsobj = <citygroups[]>(resp);
            }

      });
        ahwalmappingobj.cityGroupID = citygroupsobj[0].cityGroupID;
        ahwalmappingobj.shiftID = parseInt(this.selectedShift,10);
        ahwalmappingobj.patrolRoleID = parseInt(this.selectedRole,10);
        if(this.ahwalMappingAddMethod === 'UPDATE'){
            ahwalmappingobj.ahwalMappingID = this.selahwalmappingid;
            this.svc.UpDateAhwalMapping(ahwalmappingobj).subscribe(resp =>
                {
                    this.clearpersonpopupvalues();
                    this.ahwalMapping_Add_status_label = resp;
                 this.loadData();
              });
        }
        else {
            this.svc.AddAhwalMapping(ahwalmappingobj,this.userobj).subscribe(resp =>
                {
                    this.clearpersonpopupvalues();
                    let ol:operationLog = new operationLog();
                    ol= <operationLog>resp;
                    this.ahwalMapping_Add_status_label = ol.text;
                 this.loadData();
              });
        }
}
else if(parseInt(this.selectedRole, 10 ) === handler_ahwalMapping.PatrolRole_Associate)
{

if(this.selectedAssociateMapId === null){
    this.ahwalMapping_Add_status_label  = 'يرجى اختيار الشفت';
    return;
}
let ahwalMappingForAssociateobj:ahwalmapping;
this.svc.GetMappingByID(parseInt(this.selectedAssociateMapId , 10 ),this.userid).subscribe(resp =>
    {

        if (resp !== null)
        {
            ahwalMappingForAssociateobj = <ahwalmapping>resp;
        }

  });

if(ahwalMappingForAssociateobj !== null)
{
  if (personobj.personID === ahwalMappingForAssociateobj.personID){
    this.ahwalMapping_Add_status_label = 'المرافق نفس الفرد، ماهذا ؟؟؟؟';
    return;
}
ahwalmappingobj.ahwalID = ahwalMappingForAssociateobj.ahwalID;
ahwalmappingobj.personID = ahwalMappingForAssociateobj.personID;
ahwalmappingobj.sectorID = ahwalMappingForAssociateobj.sectorID;
ahwalmappingobj.cityGroupID = ahwalMappingForAssociateobj.cityGroupID;
ahwalmappingobj.shiftID = ahwalMappingForAssociateobj.shiftID;
ahwalmappingobj.patrolRoleID = parseInt(this.selectedRole , 10);
if(this.ahwalMappingAddMethod === 'UPDATE'){
    ahwalmappingobj.ahwalMappingID = this.selahwalmappingid;
    this.svc.UpDateAhwalMapping(ahwalmappingobj).subscribe(resp =>
        {
            this.clearpersonpopupvalues();
            this.ahwalMapping_Add_status_label = resp;
         this.loadData();
      });
}
else{
    this.svc.AddAhwalMapping(ahwalmappingobj,this.userobj).subscribe(resp =>
        {
            this.clearpersonpopupvalues();
            let ol:operationLog = new operationLog();
            ol= <operationLog>resp;
            this.ahwalMapping_Add_status_label = ol.text;
         this.loadData();
      });
}

}
}
else
{
    console.log('else');
    if(this.selectedShift === null)
    {
        this.ahwalMapping_Add_status_label = 'يرجى اختيار الشفت';
        return;
    }
    ahwalmappingobj.ahwalID=personobj.ahwalID;
    if(this.selectedSector === null)
    {
        this.ahwalMapping_Add_status_label = 'يرجى اختيار القطاع';
        return;
    }

    if(this.selectedCity === null)
    {
        this.ahwalMapping_Add_status_label ='يرجى اختيار المنطقة';
        return;
    }
    ahwalmappingobj.sectorID= parseInt(this.selectedSector , 10);
    ahwalmappingobj.shiftID = parseInt(this.selectedShift , 10);
    ahwalmappingobj.cityGroupID = parseInt(this.selectedCity,10) ;
    ahwalmappingobj.patrolRoleID = parseInt(this.selectedRole,10);
    ahwalmappingobj.personID =  personobj.personID;
    console.log(this.ahwalMappingAddMethod );
    if(this.ahwalMappingAddMethod === 'UPDATE'){
        ahwalmappingobj.ahwalMappingID = this.selahwalmappingid;
        this.svc.UpDateAhwalMapping(ahwalmappingobj).subscribe(resp =>
            {
                this.clearpersonpopupvalues();
                this.ahwalMapping_Add_status_label = resp;
             this.loadData();
          });
    }
    else{
        console.log('insert');
        console.log(ahwalmappingobj );
        this.svc.AddAhwalMapping(ahwalmappingobj,this.userobj).subscribe(resp =>
            {
                this.clearpersonpopupvalues();
                let ol:operationLog = new operationLog();
                ol= <operationLog>resp;
                this.ahwalMapping_Add_status_label = ol.text;
             this.loadData();
          });
    }
}

}

clearpersonpopupvalues()
{

    this.selectPerson_Mno = null;
    this.selectedShift = null;

   this.associatePersonMno = null;
    this.selectedCity = null;
    this.selectedAssociateMapId = null;
   /*  */
    this.ahwalMapping_Add_status_label = '';

    this.shiftvisibile = false;
    this.sectorvisibile = false;
   this.cityvisibile = false;
    this.associatevisibile = false;

    this.selectedSector = null;
   this.selectedRole = null;
    //this.searchInput.nativeElement.visible = false;
    //console.log('searchinput ' + this.searchInput);
}



RwPopupClick(e)
{
    var component = e.component,
    prevClickTime = component.lastClickTime;
component.lastClickTime = new Date();
if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
    //Double click code

}
else {
   this.selectPerson_Mno = e.values[0];
}

}



Rwclick(e)
{
   /* var component = e.component,
    prevClickTime = component.lastClickTime;
    component.lastClickTime = new Date();
    if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {

             this.CallDblClick();
    }*/
    this.styleExp='inline';
}

RwAssociatePopupClick(e)
{
    var component = e.component,
    prevClickTime = component.lastClickTime;
component.lastClickTime = new Date();
if (prevClickTime && (component.lastClickTime - prevClickTime < 300)) {
    //Double click code

}
else {
 //  this.associatePersonMno = e.values[0];
 console.log(e.values[0]);
 this.associatePersonMno = e.values[0];
}

}

RwPatrolCheckPopupClick(e)
{
    console.log(e);
    //console.log(e.data.patrolid);
    this.selCheckInOutPatrolPltNo = e.data.platenumber;
}

RwHandHeldCheckPopupClick(e)
{
this.selCheckInOutHHeldSerialNo =  e.data.serial;
}

CallDblClick()
{
    this.checkInOutPopupVisible=true;
    this.AhwalMapping_CheckInOut_ID = this.selahwalmappingid;

   // this.ShowCheckInOutPopdtls();
 }



CloseCheckInoutPopup(){
  this.checkInOutPopupVisible = false;

}
AhwalMapping_CheckInButton_Click(e)
{
    if(this.selCheckInOutPersonMno === null)
    {
    this.ahwalMapping_CheckInOut_StatusLabel = 'يرجى اختيار الفرد';
    }
   
    if(this.selCheckInOutPatrolPltNo === null)
    {
    this.ahwalMapping_CheckInOut_StatusLabel = 'يرجى اختيار الدورية';
    }

 
    if(this.selCheckInOutHHeldSerialNo == null)
    {
      this.ahwalMapping_Add_status_label = 'يرجى اختيار الجهاز';
      return;
    }

    /*this.svc.CheckInAhwalMapping(this.selCheckInOutPersonMno,this.selCheckInOutPatrolPltNo,
        this.selCheckInOutHHeldSerialNo,this.userid).subscribe(resp =>
        {
  
          
                this.ahwalMapping_Add_status_label = resp;
          
  
      });*/

    /*
    let personobj:persons = new persons();


    this.svc.GetPersonForUserForRole(this.selectedCheckInOutPerson,this.userid).subscribe(resp =>
      {

          if (resp !== [])
          {
              personobj = <persons>resp;
          }

    });

      if (personobj === null)
      {
          this.ahwalMapping_Add_status_label = 'لم يتم العثور على الفرد';
          return;

        }


  if(this.selectedCheckInOutPatrol === null)
  {
  this.ahwalMapping_CheckInOut_StatusLabel = 'يرجى اختيار الدورية';
  }

  let patrolobj:patrolcars = new patrolcars();
  this.svc.GetPatrolCarByPlateNumberForUserForRole(this.selectedCheckInOutPatrol,this.userid).subscribe(resp =>
    {

        if (resp !== [])
        {
          patrolobj = <patrolcars>resp;
        }
  });

  if (patrolobj === null)
  {
      this.ahwalMapping_Add_status_label = 'لم يتم العثور على الدورية';
      return;

    }
  if(this.selectedCheckInOutHandHeld == null)
  {
    this.ahwalMapping_Add_status_label = 'يرجى اختيار الجهاز';
    return;
  }
    let handheldobj:handhelds = new handhelds();
    this.svc.GetHandHeldBySerialForUserForRole(this.selectedCheckInOutHandHeld,this.userid).subscribe(resp =>
      {

          if (resp !== [])
          {
            handheldobj = <handhelds>(resp);
          }
    });
    let personMapping ;
    this.svc.GetMappingByPersonID(this.selectedCheckInOutPerson,this.userid).subscribe(resp =>
      {

          if (resp !== [])
          {
            handheldobj = <handhelds>(resp);
          }
    });
              if (personMapping == null)
              {
                  this.ahwalMapping_CheckInOut_StatusLabel = 'لم يتم العثور على الفرد في الكشف';
                  return;
              }
               */
}

checkhandheldexpr(item)
{
    if ( item !== undefined )
    { return  item.serial ;
    }
}

checkInassociateExpr(item)
{
    if ( item !== undefined )
    { return  item.milnumber ;
    }
}

RwPersonCheckPopupClick(e)
{
this.selCheckInOutPersonMno = e.data.milnumber;

}
}
