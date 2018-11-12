import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonService } from '../../../services/common.service';
import { DxDataGridComponent } from 'devextreme-angular'
import notify from 'devextreme/ui/notify';
import { patrolcars } from '../../../models/patrolcars';
import SelectBox from 'devextreme/ui/select_box';
import { ModalService } from '../../../services/modalservice';

@Component({
  selector: 'app-patrolcars',
  templateUrl: './patrolcars.component.html',
  styleUrls: ['./patrolcars.component.css']
})



export class PatrolcarsComponent implements OnInit {
  @ViewChild(DxDataGridComponent) dataGrid: DxDataGridComponent;
  data: any;

  loadingVisible = false;
  selahwalid: number = -1;
  rentalchk: number = 0;
  defectchk: number = 0;
  typesrc: any;
  dataSource: any;
  devicetypesrc: any;
  typelist: any;
  seltypelist: any;
  selectBox2: any;
  items: any;
  popupVisible: any = false;
  pltnumber: string = '';
  vehtype: string = 'xx';
  model: string = '';
  vinnumber: string = '';
  defective: number = 0;
  rental: number = 0;
  barcode: string = '';
  modaltitle: string = '';
  hdntrans: string = '';
  patrolid: number = 0;
  public patrolcarobj: patrolcars = new patrolcars();


  constructor(private svc: CommonService, private modalService: ModalService, private cd: ChangeDetectorRef) {
    console.log('constructor');
    this.populatetypelist();
    this.showLoadPanel();

  }


  populatetypelist() {
    this.svc.GetPatrolCarTypes().subscribe(resp => {
      console.log('types  ' + resp)
      this.typelist = JSON.parse(resp);
    },
      error => {

      });
  }

  ContentReady2(e) {
    if (!e.component.__isInitialized) {
      e.component.__isInitialized = true;

    }
  }


  onValueChangeOfSelectbox(e) {

    if (e.name === 'selectedItem') {

      console.log(e.value.text);
      if (e.value.text === '') {
        console.log('clear filters');
        this.dataGrid.instance.clearFilter();
      } else {
        console.log('filters contains');
        this.dataGrid.instance.filter(['type', 'contains', e.value.text]);
      }

    }

  }

  onShown() {
    setTimeout(() => {
      this.loadingVisible = false;
    }, 3000);
  }
  typeselboxtoggled(e) {

  }
  showLoadPanel() {
    this.loadingVisible = true;
  }

  ngOnInit() {
    console.log('Initialized');
    this.populatetypelist();
    this.LoadData();
  }

  LoadData() {
    let userid: string = window.localStorage.getItem('UserID');
    this.svc.GetPatrolCarList(this.selahwalid, parseInt(userid)).subscribe(resp => {

      //this.dataSource = JSON.parse(resp);
      //console.log('resp' + this.dataSource);
      //this.dataGrid.dataSource = this.dataSource;


      //this.dataGrid.instance.refresh();
      //this.populatetypelist();
      //this.dataGrid.instance.refresh();
    },
      error => {

      });

  }

  onToolbarPreparing(e) {
    let AhwalLst: any = [];
    AhwalLst = JSON.parse(window.localStorage.getItem('Orgs'));
    e.toolbarOptions.items.unshift({
      location: 'before',
      template: 'الأحوال'
    }, {
        location: 'before',
        widget: 'dxSelectBox',
        options: {
          width: 200,
          items: AhwalLst,
          displayExpr: 'text',
          valueExpr: 'value',
          value: 1,
          onValueChanged: this.groupChanged.bind(this)
        }
      }, {
        location: 'after',
        widget: 'dxButton',
        options: {
          icon: 'plus',
          onClick: this.ShowAddPopup.bind(this)
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

  groupChanged(e) {
    this.selahwalid = e.value;
    this.LoadData();
  }



  refreshDataGrid() {
    this.LoadData();

  }

  cleardata() {

    this.patrolcarobj = null;
    this.patrolcarobj = new patrolcars();
    /* this.patrolcarobj.ahwalid =  -1;
     this.patrolcarobj.barcode = '';
     this.patrolcarobj.defective =  -1;
     this.patrolcarobj.platenumber =  '';
     this.patrolcarobj.type =  '';
     this.patrolcarobj.model =  '';
     this.patrolcarobj.rental = -1;
     this.patrolcarobj.patrolid =  -1;
     this.patrolcarobj.vinnumber =  '';*/
    // console.log('clear' + this.patrolcarobj.ahwalid);
  }

  PopupInitialize(e) {

    this.cleardata();

    this.cleardefaultvalues();

  }

  cleardefaultvalues() {
    this.pltnumber = '';
    this.model = '';
    this.vinnumber = '';
    this.rentalchk = 0;
    this.defectchk = 0;
    this.patrolid = 0;
    this.seltypelist = 'xx';
  }
  RowAdd(e) {

    this.cleardata();
    this.patrolcarobj.ahwalid = this.selahwalid;
    this.patrolcarobj.barcode = 'PAT' + this.pltnumber;
    this.patrolcarobj.defective = this.defectchk;
    this.patrolcarobj.platenumber = this.pltnumber;
    this.patrolcarobj.typecode = this.seltypelist;
    this.patrolcarobj.model = this.model;
    this.patrolcarobj.rental = this.rentalchk;
    this.patrolcarobj.vinnumber = this.vinnumber;
    this.svc.AddPatrolCar(this.patrolcarobj).subscribe(resp => {

      this.LoadData();
    },
      error => {

      });

    this.cleardata();
    this.cleardefaultvalues();
    // this.modalService.close('custom-modal-1');
    this.popupVisible = false;
    notify(' Record Added SuccessFully', 'success', 600);
  }

  checkBoxToggled(e) {
    //console.log(e.value);
    if (e === true) {
      this.rentalchk = 1;
    }
    else {
      this.rentalchk = 0;
    }

  }

  seltypechange(e) {
    //console.log(e);
    this.seltypelist = e.value;
  }

  chkdeftoggle(e) {
    //console.log('check' + e);
    if (e === true) {
      this.defectchk = 1;
    }
    else {
      this.defectchk = 0;
    }

  }

  RowUpdate(e) {
    console.log('update' + e);
    console.log('update' + this.defectchk);

    this.cleardata();
    this.patrolcarobj.ahwalid = this.selahwalid;
    this.patrolcarobj.barcode = 'PAT' + this.pltnumber;
    this.patrolcarobj.defective = this.defectchk;
    this.patrolcarobj.platenumber = this.pltnumber;
    this.patrolcarobj.typecode = this.seltypelist;
    this.patrolcarobj.model = this.model;
    this.patrolcarobj.rental = this.rentalchk;
    this.patrolcarobj.vinnumber = this.vinnumber;
    this.patrolcarobj.patrolid = this.patrolid;
    this.svc.UpdatePatrolCar(this.patrolcarobj).subscribe(resp => {
      this.LoadData();
    },
      error => {

      });
    this.cleardata();
    this.cleardefaultvalues();
    // this.modalService.close('custom-modal-1');
    this.popupVisible = false;
    notify(' Record Updated  SuccessFully', 'success', 600);

  }

  RowDelete(e) {
    this.cleardata();
    this.patrolcarobj.patrolid = e.data.patrolid;
    this.svc.DeletePatrolCar(this.patrolcarobj).subscribe(resp => {

      console.log('resp' + resp);
      this.LoadData();
    },
      error => {

      });
  }

  showInfo() {

    this.popupVisible = true;
  }

  ShowAddPopup() {
    this.hdntrans = 'I';

    this.modaltitle = 'Add Patrol Car';
    this.cleardefaultvalues();
    //this.modalService.open('custom-modal-1');
    this.popupVisible = true;


  }

  ShowUpdatePopup(e, dt) {
    //this.cd.detectChanges();
    console.log(dt.rental);
    this.hdntrans = 'U';
    this.pltnumber = dt.platenumber;
    this.model = dt.model;
    this.rentalchk = dt.rental;
    this.defectchk = dt.defective;
    this.vinnumber = dt.vinnumber;
    this.patrolid = dt.patrolid;
    this.seltypelist = dt.typecode;
    this.modaltitle = 'Update Patrol Car';
    this.popupVisible = true;
    //this.modalService.open('custom-modal-1');
    //this.cd.detectChanges();

    //this.cd.detectChanges();


  }


  DelRecord(e, rindex) {
    this.dataGrid.instance.deleteRow(rindex);
  }

  ClosePopup() {
    this.popupVisible = false;
    //this.modalService.close('custom-modal-1');
  }

}

