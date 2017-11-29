# XlsLoaderr
Загрузчик данных из Excel по шаблону

Т.к. код выложен для ознакомления, в проекте отсутствуют определенные компоненты и dll, поэтому он не будет работать.

1) Проект WebApi
    Web компонент который получает с клиента файл excel, данные из которого нужно загрузить в БД.

--XlsController.XlsUpload
  получает данные с клиента, загружает необходимые справочники из БД (DicService.GetDrillOrderDictionarys), запускает загрузчик данных DrillOrderLoader, отпраляет ответ на клиент
  
2) Проект XlsLoader
    Основной компонент который осуществляет загрузку данных из excel в БД
    
 --DrillOrderLoader.UploadDrillOrderExcelAsync
  инициализация DrillOrderLoader и запуск основного метода загрузки UploadDrillOrderExcel
  
 --DrillOrderLoader.UploadDrillOrderExcel
 загрузка 2-х шаблонов titleSheet и (остальные шаблоны в проекте не представлены)
 
 --OpenXml.GetSheetData
 получить данные листа excel по его имени
    
 --Title.PrepareLoadData
 получение и компановка данных с листа excel, подготовка модели данных для выгрузки в БД
 
 --DrillOrderService.LoadExcelTitleWorkOrder
 отправка данных в БД
 
