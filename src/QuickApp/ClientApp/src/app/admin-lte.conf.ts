export const adminLteConf = {
  skin: 'blue',
  sidebarLeftMenu: [
    {label: 'Home', route: 'home', iconClasses: 'fa fa-road'},
    {label: 'الصيانه', iconClasses: 'fa fa-th-list', children: [
        {label: 'الدوريات', route: 'maintainence/patrolcars',iconClasses:'fa fa-automobile'},
        {label:'تقارير الاستلام والتسليم الأجهزة', route: 'maintainence/patrolcarsinventory',iconClasses:'fa fa-calendar'},
        {label: 'الأجهزة', route: 'maintainence/handhelds',iconClasses:'fa fa-fax'},
        {label: 'تقارير الاستلام والتسليم الدوريات', route: 'maintainence/handheldsinventory',iconClasses:'fa fa-calendar'},
        {label: 'Accessories', route: 'maintainence/accessories'},
        {label: 'Accessory Inventory', route: 'maintainence/accessoriesinventory',iconClasses:'fa fa-calendar'}
      ]},
      {label: 'الأحوال', iconClasses: 'fa fa-eye', children: [
        {label: 'كشف التوزيع', route: 'dispatcher/dispatcher',iconClasses:'fa fa-calendar'},
        {label: 'الأفراد', route: 'dispatcher/persons',iconClasses:'fa fa-user-secret'}
      ]},
    {label: 'Map Viewer', route: 'map', iconClasses:'fa fa-globe'},
    {label: 'Settings', route: 'settings', iconClasses: 'fa fa-gear'}

    
  ]
};
