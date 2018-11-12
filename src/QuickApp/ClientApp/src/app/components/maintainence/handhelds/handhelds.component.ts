import { Component, OnInit ,ViewChild} from '@angular/core';
import { CommonService } from '../../../services/common.service';
import { DxDataGridComponent } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';
import {handhelds} from '../../../models/handhelds';


@Component({
  selector: 'app-handhelds',
  templateUrl: './handhelds.component.html',
  styleUrls: ['./handhelds.component.css']
})
export class HandheldsComponent implements OnInit {

  @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
  loadingVisible = false;
  selahwalid:number = -1;
  rentalchk:number = 0;
  defectchk:number = 0;
  typesrc:any;
  dataSource: any;
  devicetypesrc:any;


  public handheldobj:handhelds = new handhelds();


  constructor(private svc:CommonService) {

    this.showLoadPanel();
   // this.typesrc = JSON.parse(window.localStorage.getItem("devicetypes"));
   }



   onShown() {
    setTimeout(() => {
        this.loadingVisible = false;
    }, 3000);
  }
  typeselboxtoggled(e)
  {

  }
  showLoadPanel() {
    this.loadingVisible = true;
  }

  ngOnInit() {

    this.LoadData();
  }

LoadData()
{
  this.svc.GethandheldsList().subscribe(resp =>
    {

       this.dataSource = JSON.parse(resp);
      console.log('resp' + resp);
      this.dataGrid.dataSource = this.dataSource;
      this.dataGrid.instance.refresh();

  },
    error => {

    });



}
onToolbarPreparing(e) {
  let strt :any=[];
strt =JSON.parse(window.localStorage.getItem("Orgs"));
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
        value:'1',
        onValueChanged: this.groupChanged.bind(this)
          }
      }, {
          location: 'after',
          widget: 'dxButton',
          options: {
              icon: 'refresh',
              onClick: this.refreshDataGrid.bind(this)
          }
      });
}

groupChanged(e) {
  this.selahwalid = e.value;
 this.LoadData();
}

ContextMenuprepare(e)
{
   if (e.row.rowType === "data") {
    e.items = [{
      text: "جديد",
      value:e.row.rowIndex,
      onItemClick: this.ContextMenuClick.bind(this)
  },
  {
      text: "تعديل",
      value:e.row.rowIndex,
      onItemClick:this.ContextMenuClick.bind(this)
  },
  {
      text: "حذف",
      value:e.row.rowIndex,
      onItemClick: this.ContextMenuClick.bind(this)
  },
  {
    text: "تقرير",
    items:[{ text: "Excel",
  value:e.row.rowIndex,
  onItemClick: this.ContextMenuClick.bind(this)
}]

}];

  }
}

ContextMenuClick(e)
{
  console.log(e);
  if (e.itemIndex === 0)
  {
    this.dataGrid.instance.insertRow();
  }
  if (e.itemIndex === 1)
  {
    this.dataGrid.instance.editRow(e.itemData.value);
  }

  if (e.itemIndex === 2)
  {
    this.dataGrid.instance.deleteRow(e.itemData.value);
  }

  if (e.itemIndex === 4)
  {
    this.dataGrid.instance.exportToExcel(false);
  }
}


refreshDataGrid() {
  this.LoadData();
  //this.dataGrid.instance.refresh();
}

cleardata()
{
  this.handheldobj = null;
  this.handheldobj= new handhelds();
}

PopupInitialize(e)
{
  console.log('popupini');
  this.cleardata();
  this.cleardefaultvalues();
  if (e.parentType === 'dataRow' && e.dataField === 'barcode')
  {
  e.cancel = true;
  e.component.columnOption("barcode", "formItem", "{visible: false}");
    }
}
cleardefaultvalues()
{

  this.defectchk = 0;
}
chkdeftoggle(e)
{
  //console.log(e.value);
  if( e.value === true)
  {
    this.defectchk = 1;
  }
  else{
    this.defectchk = 0;
  }

}
RowAdd(e)
{

  this.cleardata();
  this.handheldobj.ahwalid =  this.selahwalid;
  this.handheldobj.barcode =  e.data.barcode;
  this.handheldobj.defective =  this.defectchk;
  this.handheldobj.serial =  e.data.serial;


  this.svc.Addhandhelds(this.handheldobj).subscribe(resp =>
    {
      console.log('resp' + resp);
     this.LoadData();
  },
    error => {

    });
    this.cleardata();
    this.cleardefaultvalues();

  notify(' Record Added SuccessFully', 'success', 600);
}




RowUpdate(e)
{
  console.log('update' + e.data);
  this.handheldobj.ahwalid =  this.selahwalid;
  this.handheldobj.barcode =  e.data.barcode;
  this.handheldobj.defective =  this.defectchk;
  this.handheldobj.serial =  e.data.serial;
  this.handheldobj.handheldid = e.data.handheldid;
  this.svc.Updatehandhelds(this.handheldobj).subscribe(resp =>
    {

     this.LoadData();
  },
    error => {

    });
}

RowDelete(e)
{
  this.cleardata();

  this.handheldobj.handheldid =  e.data.handheldid;
 console.log('delete' + e.data);
  this.svc.Deletehandhelds(this.handheldobj).subscribe(resp =>
    {

      console.log('resp' + resp);
     this.LoadData();
  },
    error => {

    });
}



}
