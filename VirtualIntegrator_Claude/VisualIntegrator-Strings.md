# Visual Integrator - String/Resource/Secret Audit

Generated: 2026-11-17 12:06:34

Scan over managed Circon assemblies in `C:\Program Files (x86)\Circon\BIN`.
Per assembly: 1) string literal constants from IL metadata, 2) regex scan of raw bytes for SQL / connection strings / URLs / paths / registry keys / emails.

## vilns32.exe

### String literal constants (6)

``````
Circon.VisualIntegrator.NavigateTree.Default = _default
Circon.VisualIntegrator.NavigateTree.DefaultGraphic = _default.vbd
Circon.VisualIntegrator.NavigateTree.DefaultGraphicSearch = _default.*
Circon.VisualIntegrator.Wizard.WizardDeviceTemplatesForm.appDeviceTemplateBlank = Blank Device
Circon.VisualIntegrator.Wizard.WizardDeviceTemplatesForm.lonMarkObjectTemplateBlank = Blank Functional Block
Circon.VisualIntegrator.Wizard.WizardDeviceTemplatesForm.unassignedTemplate = <unassigned>
``````

### SQL (SELECT) matches (2)

``````
-- A list of all Windows versions that this application is designed to work with. Windows will automatically select the most compatible environment.-->\r\n        <!-- Windows 10 -->\r\n        <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}"/>\r\n        <!-- Windows 8.1 -->\r\n        <sup...(truncated)
', please select another nameᤁlblDocuments耀⚙匀攀氀攀挀琀 攀砀椀猀琀椀渀最 琀攀洀瀀氀愀琀攀 琀漀 漀瘀攀爀眀爀椀琀攀 漀爀 攀渀琀攀爀 渀愀洀攀 昀漀爀 渀攀眀 琀攀洀瀀氀愀琀攀 戀攀氀漀眀㨀
``````

### SQL (INSERT/UPDATE/DELETE) matches (1)

``````
update to devicesᄀMainLeftༀMainTopጀMainWidthᔀMainHeightጀLayoutv40ༀRecycleഀAlarmsༀBrowser℀BrowserShowUnitsᴀOnFormClosing 䴀Please close the Editor before exiting耀䇡 爀甀渀渀椀渀最 琀愀猀欀 栀愀猀 渀漀琀 挀漀洀瀀氀攀琀攀搀Ⰰ 椀昀 礀漀甀 挀氀漀猀攀
``````

### URL matches (7)

``````
http://crl.usertrust.com/USERTrustRSACertificationAuthority.crl0v
http://ocsp.sectigo.com0
http://ocsp.usertrust.com0
http://www.circon.com/0
http://www.w3.org/2001/XMLSchema-instance
https://sectigo.com/CPS0C
https://sectigo.com/CPS0D
``````

### Email matches (1)

``````
admin@circon.com
``````

## nilns32.exe


### SQL (SELECT) matches (1)

``````
-- A list of all Windows versions that this application is designed to work with. Windows will automatically select the most compatible environment.-->\r\n\r\n      <!-- If your application is designed to work with Windows 7, uncomment the following supportedOS node-->\r\n        <!-- Windows 10 -->...(truncated)
``````

### URL matches (7)

``````
http://crl.usertrust.com/USERTrustRSACertificationAuthority.crl0v
http://ocsp.sectigo.com0
http://ocsp.usertrust.com0
http://www.circon.com/0
http://www.w3.org/2001/XMLSchema-instance
https://sectigo.com/CPS0C
https://sectigo.com/CPS0D
``````

### Email matches (1)

``````
admin@circon.com
``````

## CirconUtil.dll

### String literal constants (1)

``````
Circon.Util.RegistryLogon.lastlogonValue = Last Logged On
``````

## CirconService.dll


### URL matches (3)

``````
http://Circon.Service
http://Circon.Service.Log
http://Circon.Service.Opc
``````

## CirconData.dll


### SQL (SELECT) matches (26)

``````
(SELECT schedule_zones.id FROM schedule_zones,access_groups,access_groups_points,access_points,lns_lonmark_objects\r\n                WHERE schedule_zones.id=access_groups.schedule_zone_id AND
DELETE FROM access_device_group_actions WHERE access_device_group_id IN (SELECT id FROM access_device_groups WHERE access_group_id={0} AND device_id={1}) 脀䥅一匀䔀刀吀 䤀一吀伀 愀挀挀攀猀猀开搀攀瘀椀挀攀开最爀漀甀瀀开愀挀琀椀漀渀猀⠀氀渀猀开愀挀琀椀漀渀开椀搀Ⰰ愀挀挀攀猀猀开搀攀瘀椀挀攀开最爀漀甀瀀开椀搀⤀ 匀䔀䰀䔀䌀吀 笀　紀Ⰰ椀搀 䘀刀伀䴀 愀挀挀攀猀猀开搀攀瘀椀挀攀开最爀漀甀瀀猀 圀䠀䔀刀䔀 愀
DELETE FROM access_device_group_lockout_tests WHERE access_device_group_id IN (SELECT id FROM access_device_groups WHERE access_group_id={0} AND device_id={1}) 脀䥍一匀䔀刀吀 䤀一吀伀 愀挀挀攀猀猀开搀攀瘀椀挀攀开最爀漀甀瀀开氀漀挀欀漀甀琀开琀攀猀琀猀⠀氀渀猀开琀攀猀琀开椀搀Ⰰ愀挀挀攀猀猀开搀攀瘀椀挀攀开最爀漀甀瀀开椀搀⤀ 匀䔀䰀䔀䌀吀 笀　紀Ⰰ椀搀 䘀刀伀䴀 愀挀挀攀猀猀开搀攀瘀椀挀攀开最爀漀甀瀀猀 圀䠀䔀
FROM (SELECT access_users.id, COUNT(access_cards.id) AS count FROM access_cards AS temp_cards,access_users \r\n                    LEFT JOIN access_cards ON \r\n                    (access_users.id=access_cards.access
FROM (SELECT access_users.id, COUNT(access_cards.id) AS count FROM access_users \r\n                   LEFT JOIN access_cards ON \r\n                   (access_users.id=access_cards.access_user_id AND access_cards.dis
FROM (SELECT access_users.id, COUNT(access_cards.id) AS count FROM access_users \r\n                  LEFT JOIN access_cards ON \r\n                  (access_users.id=access_cards.access_user_id AND access_cards.disab
FROM (SELECT access_users.id, COUNT(access_cards.id) AS count FROM access_users_custom_exts,access_users_exts,access_users \r\n                   LEFT JOIN access_cards ON \r\n                   (access_users.id=acces
FROM (SELECT access_users.id, COUNT(access_cards.id) AS count FROM access_users_custom_exts,access_users_exts,access_users\r\n                   LEFT JOIN access_cards ON \r\n                   (access_users.id=access
IF EXISTS (SELECT id FROM tempdb.dbo.sysobjects WHERE id = OBJECT_ID('Tempdb..#tempalarms')AND type = 'U') DROP TABLE #tempalarms \r\n              SELECT tempalarms.* INTO #tempalarms FROM \r\n              (
IF EXISTS (SELECT id FROM tempdb.dbo.sysobjects WHERE id = OBJECT_ID('Tempdb..#tempalarms')AND type = 'U') DROP TABLE #tempalarms \r\n            SELECT tempalarms.* INTO #tempalarms FROM \r\n            (
INSERT INTO access_device_group_enable_tests(lns_test_id,access_device_group_id) SELECT {0},id FROM access_device_groups WHERE access_group_id={1} AND device_id={2} 㴀AccessDeviceGroupEnableTestsDs欀http://tempuri.org/AccessDeviceGroupEnableTestsDs.xsd茀匛䔀䰀䔀䌀吀 愀挀挀攀猀猀开搀攀瘀椀挀攀开最爀漀甀瀀开氀漀挀欀漀甀琀开琀攀
SELECT  #tempalarms.*, abs(#tempalarms.priority-5) AS zzzz FROM #tempalarms WHERE #tempalarms.access_user_id IS NULL \r\n            UNION \r\n            SELECT  #tempalarms.*, abs(#tempalarms.priority-5) AS zzz
SELECT  alarms.*,device_events.device_id,device_events.time_stamp,device_events.priority,device_events.code,device_events.object,device_events.snvtId,device_events.data,device_events.description,device_events
SELECT  alarms.id,device_events.access_user_id\r\n                FROM alarm_colors,groups_alarm_ack_codes,sessions,groups_sites,users_groups,alarm_priorities,alarm_states,alarm_categories,alarm_codes,alarms WI
SELECT @count1 = COUNT(#tempalarms.id) FROM #tempalarms WHERE #tempalarms.access_user_id IS NULL \r\n            \r\n            SELECT @count2 = COUNT(#tempalarms.id) FROM sessions,groups_partitions,users_group
SELECT @count1+@count2ᄁAlarmsDs㼀http://tempuri.org/AlarmsDs.xsdጀascendingᴀ{0}.time_stamp⌀{0}.site_fullnameጀ{0}.state✀abs({0}.priority-5)ᤁ{0}.categoryᴀ{0}.alarm_codeἀ{0}.device_path⼀{0}.lonmark_object_nameἀ{0
SELECT @name,parent_zone.id,parent_zone.site_id \r\n                FROM access_zones AS parent_zone WHERE parent_zone.id=@parent_id\r\n              SELECT SCOPE_IDENTITY()开UPDATE access_zones SET name=@name WH
SELECT lns_devices.site_id,@device_index,@device_index,@elevator,\r\n                    lns_lonmark_objects.id FROM lns_lonmark_objects,lns_devices\r\n                    WHERE lns_lonmark_objects.object_index=
SELECT SCOPE_IDENTITY()଀@cardⴀ@access_card_format_id耀䖓堀䔀䌀 猀攀琀猀攀猀猀椀漀渀 䀀欀攀礀ഀ਀              䐀䔀䰀䔀吀䔀 䘀刀伀䴀 愀挀挀攀猀猀开挀愀爀搀猀 圀䠀䔀刀䔀 椀搀㴀䀀椀搀
SELECT SCOPE_IDENTITY()ᤀ@lns_test_id耀䒙䔀䰀䔀吀䔀 䘀刀伀䴀 愀挀挀攀猀猀开琀攀猀琀猀 圀䠀䔀刀䔀 氀渀猀开琀攀猀琀开椀搀㴀䀀氀渀猀开琀攀猀琀开椀搀 䄀一䐀 猀椀琀攀开椀搀㴀䀀猀椀琀攀开椀搀
SELECT SCOPE_IDENTITY()ᬀ@device_indexጀ@elevator耀喙倀䐀䄀吀䔀 愀挀挀攀猀猀开瀀漀椀渀琀猀 匀䔀吀 搀攀猀挀爀椀瀀琀椀漀渀㴀䀀搀攀猀挀爀椀瀀琀椀漀渀 圀䠀䔀刀䔀 愀挀挀攀猀猀开瀀漀椀渀琀猀⸀椀搀㴀䀀椀搀
SET NOCOUNT ON INSERT access_card_formats (name,access_format_type_id,size) VALUES (@name,@access_format_type_id,@size) SELECT SCOPE_IDENTITY()ⴀ@access_format_type_id଀@size夀DELETE FROM access_card_formats WHERE id=@id洀UPDATE access_card_formats SET name=@name WHERE id=@id✀AccessCardFormatsDs唀http://...(truncated)
SET NOCOUNT ON INSERT access_users (partition_id,title,first_name,middle_name,last_name,suffix,display_name,timestamp_added,timestamp_modified) SELECT @partition_id,@title,@first_name,@middle_name,@last_name,@suffix,@display_name,GETDATE(),GETDATE() SELECT SCOPE_IDENTITY()ഀ@titleᤀ@middle_nameༀ@suffi...(truncated)
SET NOCOUNT ON INSERT INTO access_device_cards (device_id,card,disabled,handicapped,expiry,pin,groups) VALUES (@device_id,@card,@disabled,@handicapped,@expiry,@pin,@groups) SELECT SCOPE_IDENTITY()ᤀ@handicappedༀ@groups脀卫䔀吀 一伀䌀伀唀一吀 伀一 䤀一匀䔀刀吀 䤀一吀伀 愀挀挀攀猀猀开搀攀瘀椀挀攀开挀愀爀搀猀 ⠀搀攀瘀椀挀攀开椀搀Ⰰ挀愀爀搀Ⰰ搀椀猀愀戀氀攀搀Ⰰ栀愀渀搀椀挀愀瀀瀀攀...(truncated)
SET NOCOUNT ON INSERT INTO access_device_groups (access_group_id,device_id,points,device_assigned_group_id) VALUES (@access_group_id,@device_id,@points,@device_assigned_group_id) SELECT SCOPE_IDENTITY()ༀ@points㌀@device_assigned_group_id脀叓䔀吀 一伀䌀伀唀一吀 伀一 䤀一匀䔀刀吀 䤀一吀伀 愀挀挀攀猀猀开搀攀瘀椀挀攀开最爀漀甀瀀猀 ⠀愀挀挀攀猀猀开最爀漀甀瀀开椀...(truncated)
SET NOCOUNT ON INSERT schedule_zones (partition_id,name) VALUES (@partition_id,@name) SELECT SCOPE_IDENTITY()ἀScheduleZonesDs䴀http://tempuri.org/ScheduleZonesDs.xsdᄀsessionsᔀSessionsDs䄀http://tempuri.org/SessionDs.xsd舀卉䔀吀 一伀䌀伀唀一吀 伀一 ഀ਀              䐀䔀䌀䰀䄀刀䔀 䀀渀攀眀开欀攀礀 䄀匀 甀渀椀焀甀攀椀搀攀渀琀椀昀椀攀爀 ഀ਀
``````

### SQL (INSERT/UPDATE/DELETE) matches (19)

``````
DELETE FROM access_group_actions WHERE access_action_id=@access_action_id AND access_group_id=@access_group_id耀䒏䔀䰀䔀吀䔀 䘀刀伀䴀 愀挀挀攀猀猀开最爀漀甀瀀开愀挀琀椀漀渀猀 圀䠀䔀刀䔀 愀挀挀攀猀猀开最爀漀甀瀀开椀搀㴀䀀愀挀挀攀猀猀开最爀漀甀瀀开椀搀
DELETE FROM access_group_lockout_tests WHERE access_test_id=@access_test_id AND access_group_id=@access_group_id耀䒛䔀䰀䔀吀䔀 䘀刀伀䴀 愀挀挀攀猀猀开最爀漀甀瀀开氀漀挀欀漀甀琀开琀攀猀琀猀 圀䠀䔀刀䔀 愀挀挀攀猀猀开最爀漀甀瀀开椀搀㴀䀀愀挀挀攀猀猀开最爀漀甀瀀开椀搀
DELETE FROM access_group_schedule_objects WHERE access_group_schedule_objects.device_id=@device_id \r\n                AND access_group_schedule_objects.schedule_zone_id NOT IN \r\n                (SELECT schedule_zo
DELETE FROM access_groups WHERE id=@id愀UPDATE access_groups SET name=@name WHERE id=@id脀䔉堀䔀䌀 猀攀琀猀攀猀猀椀漀渀 䀀欀攀礀ഀ਀             唀倀䐀䄀吀䔀 愀挀挀攀猀猀开最爀漀甀瀀猀 匀䔀吀 猀挀栀攀搀甀氀攀开稀漀渀攀开椀搀㴀䀀猀挀栀攀搀甀氀攀开稀漀渀攀开椀搀Ⰰ搀攀猀挀爀椀瀀琀椀漀渀㴀䀀搀攀猀挀爀椀瀀琀椀漀渀 圀䠀䔀刀
DELETE FROM schedule_zones WHERE id=@id挀UPDATE schedule_zones SET name=@name WHERE id=@id缀UPDATE schedule_zones SET description=@description WHERE id=@idⴀAccessGroupSchedulesDs嬀http://tempuri.org/AccessGroupSched
INSERT INTO access_device_cards (device_id,card,disabled,handicapped,expiry,pin,groups) VALUES (@device_id,@card,@disabled,@handicapped,@expiry,@pin,@groups) SELECT SCOPE_IDENTITY()ᤀ@handicappedༀ@groups脀卫䔀吀 一伀䌀伀
INSERT INTO access_device_groups (access_group_id,device_id,points,device_assigned_group_id) VALUES (@access_group_id,@device_id,@points,@device_assigned_group_id) SELECT SCOPE_IDENTITY()ༀ@points㌀@device_assigne
INSERT INTO access_points (site_id,device_index,schedule_index,elevator,lns_lonmark_object_id) \r\n             SELECT lns_devices.site_id,@device_index,@device_index,@elevator,\r\n                    lns_lonmark_ob
INSERT INTO access_tests (name,site_id,lns_test_id)\r\n                VALUES (@name,@site_id,@lns_test_id)\r\n                SELECT SCOPE_IDENTITY()ᤀ@lns_test_id耀䒙䔀䰀䔀吀䔀 䘀刀伀䴀 愀挀挀攀猀猀开琀攀猀琀猀 圀䠀䔀刀䔀 氀渀猀开琀攀猀琀开椀搀㴀䀀氀渀猀开琀攀猀
INSERT INTO access_zones (name,parent_id,site_id)\r\n              SELECT @name,parent_zone.id,parent_zone.site_id \r\n                FROM access_zones AS parent_zone WHERE parent_zone.id=@parent_id
UPDATE access_cards SET name=@name,disabled=@disabled,expiry=@expiry,pin=@pin \r\n             WHERE id=@idጀ@disabledༀ@expiryऀ@pinऀ@key脀䔗堀䔀䌀 猀攀琀猀攀猀猀椀漀渀 䀀欀攀礀ഀ਀              唀倀䐀䄀吀䔀 愀挀挀攀猀猀开挀愀爀搀猀 匀䔀吀 渀愀洀攀㴀䀀渀愀洀攀Ⰰ搀椀
UPDATE access_device_groups SET points=@points,schedule_index=NULL \r\n              WHERE access_group_id=@access_group_id AND device_id=@device_id脀唻倀䐀䄀吀䔀 愀挀挀攀猀猀开搀攀瘀椀挀攀开最爀漀甀瀀猀 匀䔀吀 瀀漀椀渀琀猀㴀䀀瀀漀椀渀琀猀Ⰰ猀挀栀攀搀甀氀攀开椀渀搀攀
UPDATE access_points SET device_index=@device_index,lns_lonmark_object_id=lns_lonmark_objects.id FROM lns_lonmark_objects WHERE lns_lonmark_objects.device_id=@device_id AND lns_lonmark_objects.object_index=@
UPDATE access_points SET partition_id=NULL WHERE id=@id洀UPDATE access_points SET zone_id=@zone_id WHERE id=@id攀UPDATE access_points SET zone_id=NULL WHERE id=@id耀䖕堀䔀䌀 猀攀琀猀攀猀猀椀漀渀 䀀欀攀礀ഀ਀              䐀䔀䰀䔀吀䔀 䘀刀
UPDATE access_users SET title=@title,first_name=@first_name,middle_name=@middle_name,last_name=@last_name, suffix=@suffix,display_name=@display_name,description=@description,timestamp_modified=GETDATE(),hand
UPDATE access_users_exts \r\n                SET phone_home=@phone_home,phone_office=@phone_office,\r\n                    phone_mobile=@phone_mobile,fax=@fax,email=@email,email_mobile=@email_mobile,
UPDATE alarms SET alarms.ack_user_id=sessions.user_id,alarms.ack_comment=@comment,alarms.state=1,alarms.ack_time_stamp=GETDATE() FROM alarms,sessions,device_events,users_groups,groups_sites,lns_devices
UPDATE alarms SET alarms.clear_user_id=sessions.user_id,alarms.clear_comment=@comment,alarms.state=2,alarms.clear_time_stamp=GETDATE() FROM alarms,sessions,device_events,users_groups,groups_sites,lns_devices
UPDATE lns_devices SET subsystem=@subsystem,name=@name,subnet=@subnet,node=@node \r\n             WHERE id=@id耀喡倀䐀䄀吀䔀 氀渀猀开搀攀瘀椀挀攀猀 匀䔀吀 愀挀挀攀猀猀开挀愀爀搀开昀漀爀洀愀琀开椀搀㴀䀀愀挀挀攀猀猀开挀愀爀搀开昀漀爀洀愀琀开椀搀 圀䠀䔀刀䔀 椀搀㴀䀀椀搀
``````

### Connection string matches (2)

``````
Data Source=TAS64\CIRCON;Initial Catalog=circon;Integrated Security=True
password=@md5hash_password AND workstations.name=@workstation AND sid IS NULL脀匷䔀䰀䔀䌀吀 甀猀攀爀猀⸀⨀Ⰰ　 䄀匀 眀椀渀搀漀眀猀开甀猀攀爀 䘀刀伀䴀 甀猀攀爀猀 圀䠀䔀刀䔀 氀漀最椀渀㴀䀀氀漀最椀渀 䄀一䐀 搀椀猀愀戀氀攀开愀挀挀漀甀渀
``````

## CirconDataForms.dll

### String literal constants (2)

``````
Circon.Data.Forms.AccessActions.ActionExtensionPrefix = Circon_ActionName_
Circon.Data.Forms.AccessTests.TestExtensionPrefix = Circon_TestName_
``````

## CirconDataLinqForms.dll

### String literal constants (9)

``````
Circon.Data.Forms.AlarmPrinterPropertySheet.noPrinter = <none>
Circon.Data.Forms.CirconLogDevice.dataLogExtension = Circon_DataLogging
Circon.Data.Forms.CirconLogDevice.EventFileInfo = Events
Circon.Data.Forms.CirconLogDevice.TrendFileInfo = Trend00
Circon.Data.Forms.LnsXml.CirconExtensionPrefix = Circon_
Circon.Data.Forms.LnsXml.EchelonLmwExtensionAllocFb = Echelon_LMW_AllocatedFbs
Circon.Data.Forms.LnsXml.interfaceMismatchMessage = The interface of the destination device does not match that of the import data
Circon.Data.Forms.UsersListView.SID_PREFIX = LDAP://<SID=
Circon.Reports.ScheduleReportForm.sqlCmd = SELECT\r\n                schedule_zones.id,\r\n                schedule_zones.name,\r\n                schedule_zones.partition_id,\r\n                schedule_zones.schedule_zone_parent_id,\r\n     ...(truncated)
``````

## CirconForms.dll

### String literal constants (28)

``````
Circon.Forms.ActiveDirectory.CLIPBOARD_FORMAT.CFSTR_DSOP_DS_SELECTION_LIST = CFSTR_DSOP_DS_SELECTION_LIST
Circon.Forms.CirconActivate.AILicenseFile = AccessIntegrator.dat
Circon.Forms.CirconActivate.BILicenseFile = BACnetIntegrator.dat
Circon.Forms.CirconActivate.NILicenseFile = NetworkIntegrator.dat
Circon.Forms.CirconActivate.VILicenseFile = TurboActivate.dat
Circon.Forms.CLSIDGuid.FileOpenDialog = DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7
Circon.Forms.CLSIDGuid.FileSaveDialog = C0B4E2F3-BA21-4773-8DBA-335EC946EB8B
Circon.Forms.CLSIDGuid.KnownFolderManager = 4df0c730-df9d-4ae3-9153-aa6b82e9795a
Circon.Forms.CLSIDGuid.ProgressDialog = F8383852-FCD3-11d1-A6B9-006097DF5BD4
Circon.Forms.HexTextBox.ValidChars = abcdefABCDEF0123456789
Circon.Forms.IIDGuid.IFileDialog = 42f85136-db7e-439c-85f1-e4075d135fc8
Circon.Forms.IIDGuid.IFileDialogControlEvents = 36116642-D713-4b97-9B83-7484A9D00433
Circon.Forms.IIDGuid.IFileDialogCustomize = e6fdd21a-163f-4975-9c8c-a69f1ba37034
Circon.Forms.IIDGuid.IFileDialogEvents = 973510DB-7D7F-452B-8975-74A85828D354
Circon.Forms.IIDGuid.IFileOpenDialog = d57c7288-d4ad-4768-be02-9d969532d960
Circon.Forms.IIDGuid.IFileSaveDialog = 84bccd23-5fde-4cdb-aea4-af64b83d78ab
Circon.Forms.IIDGuid.IKnownFolder = 38521333-6A87-46A7-AE10-0F16706816C3
Circon.Forms.IIDGuid.IKnownFolderManager = 44BEAAEC-24F4-4E90-B3F0-23D258FBB146
Circon.Forms.IIDGuid.IModalWindow = b4db1657-70d7-485e-8e3e-6fcb5a5c1802
Circon.Forms.IIDGuid.IProgressDialog = EBBC7C04-315E-11d2-B62F-006097DF5BD4
Circon.Forms.IIDGuid.IPropertyStore = 886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99
Circon.Forms.IIDGuid.IShellItem = 43826D1E-E718-42EE-BC55-A1E261C37BFE
Circon.Forms.IIDGuid.IShellItemArray = B63EA76D-1F85-456F-A19C-48159EFA858B
Circon.Forms.KFIDGuid.ComputerFolder = 0AC0837C-BBF8-452A-850D-79D08E667CA7
Circon.Forms.KFIDGuid.Documents = FDD39AD0-238F-46AF-ADB4-6C85480369C7
Circon.Forms.KFIDGuid.Favorites = 1777F761-68AD-4D8A-87BD-30B759FA33DD
Circon.Forms.KFIDGuid.Profile = 5E6C858F-0E22-4760-9AFE-EA3317B67173
Circon.Forms.MRUManager.regEntryName = file
``````

### SQL (SELECT) matches (1)

``````
Prompts the user to select a folder.
``````

### URL matches (1)

``````
http://www.circon.com
``````

### Email matches (1)

``````
support@circon.com
``````

## CirconLns.dll

### String literal constants (22)

``````
Circon.Lns.LnsAppDevice.CirconInterfaceName = CirconInterface
Circon.Lns.LnsAppDevice.EchelonLmwVirtualFbNameKey = Echelon_LMW_VirtualFbName
Circon.Lns.LnsDeviceTemplate.CirconManufacturerId = 000018
Circon.Lns.LnsDsFormat.SCPT = SCPT
Circon.Lns.LnsDsFormat.SNVT = SNVT
Circon.Lns.LnsDsFormat.UCPT = UCPT
Circon.Lns.LnsDsFormat.UNVT = UNVT
Circon.Lns.LnsFormatSpec.longDegreesC = degrees C
Circon.Lns.LnsFormatSpec.longDegreesCelsius = degrees Celsius
Circon.Lns.LnsFormatSpec.longDegreesF = degrees F
Circon.Lns.LnsFormatSpec.longPercent = % of full level
Circon.Lns.LnsFormatSpec.longPercent2 = % of full scale
Circon.Lns.LnsLonMarkObjectAllocation.EchelonLmwAllocatedFb = Echelon_LMW_AllocatedFbs
Circon.Lns.LnsMessageTag.connectionKeyPrefix = Circon_MTConn_
Circon.Lns.LnsNetworkInterfaces.LightWeightInterfaceName = Internet
Circon.Lns.LnsNetworkVariable.connectionKey = Circon_Connection
Circon.Lns.LnsNetworkVariable.legacyConnectionKeyPrefix = Circon_NVConn_
Circon.Lns.LnsObjectServer.InvalidNameChars = *?.\:/"
Circon.Lns.LnsPluginRegistryKeys.pluginRegistryKeyName = SOFTWARE\LonWorks\LCA\Plug-Ins
Circon.Lns.LnsSystem.AuthenticationDisabledKey = FFFFFFFFFFFF
Circon.Lns.LnsSystem.NeuronIdNull = 000000000000
Circon.Lns.LnsSystem.validHexDigits = 0123456789ABCDEFabcdef
``````

### URL matches (3)

``````
http://example.org/lns
http://example.org/lns5
http://www.w3.org/2001/XMLSchema
``````

## CirconLnsDevices.dll

### String literal constants (4)

``````
Circon.Lns.Devices.ScheduleDeviceTemplate.eventsPropertyName = UCPTscheduleEvent
Circon.Lns.Devices.ScheduleDeviceTemplate.schedulePropertyName = UCPTschedule
Circon.Lns.Devices.ScheduleLonMarkObject.scheduleEventNamesExtension = Circon_ScheduleEventNames
Circon.Lns.Devices.ScheduleLonMarkObject.scheduleNamesExtension = Circon_ScheduleNames
``````

## CirconLnsDevicesForms.dll


## CirconLnsForms.dll

### String literal constants (8)

``````
Circon.Lns.Forms.LnsClipboard.CirconExtensionPrefix = Circon_
Circon.Lns.Forms.LnsClipboard.EchelonExtension = Echelon_LMW_AllocatedFbs
Circon.Lns.Forms.OpenDialupiLonNetworkForm.xDriverPrefix = X.Circon.
Circon.Lns.Forms.OpenLocalNetworkForm.xDriverPrefix = X.Circon.
Circon.Lns.Forms.OpenSites.xDriverPrefix = X.Circon.
Circon.Lns.Forms.OpenSitesForm.local = Local
Circon.Lns.Forms.OpenSitesForm.remoteFull = Remote Full Client
Circon.Lns.Forms.OpenSitesForm.remoteLightWeight = Remote Lightweight Client
``````

### SQL (INSERT/UPDATE/DELETE) matches (1)

``````
Update Interval:㄀lblUpdateIntervalSecondsༀsecondsᴀlblPolIntervalᴀPoll Interval:㜀lblDiscoveryIntervalSeconds⤀lblDiscoveryInterval㴀Run background discovery everyᜀlblDatabase✀lblNetworkInterface⌀lblServerLocati
``````

## CirconLonWorks.dll

### String literal constants (1)

``````
Circon.LonWorks.SpidData.description = Standard Program ID Data
``````

## CirconNvt.dll


## CirconLdrf.dll

### String literal constants (7)

``````
Circon.Ldrf.LdrfFmtFile.programIdPrefix = set program_id
Circon.Ldrf.LdrfFmtFile.selectorPrefix = set selector
Circon.Ldrf.LdrfUnits.LongDegreesC = degrees C
Circon.Ldrf.LdrfUnits.LongDegreesCelsius = degrees Celsius
Circon.Ldrf.LdrfUnits.LongDegreesF = degrees F
Circon.Ldrf.LdrfUnits.LongPercentOfFullLevel = % of full level
Circon.Ldrf.LdrfUnits.LongPercentOfFullScale = % of full scale
``````

## CirconLinq.dll

### String literal constants (1)

``````
Circon.Data.Linq.CirconDatabase.CurrentVersion = 2.88
``````

### SQL (INSERT/UPDATE/DELETE) matches (1)

``````
UPDATE schedule_events SET schedule_events.expired=1 \r\n                                WHERE (schedule_events.end_year != 0) AND (schedule_events.end_month != 0) AND (schedule_events.end_day != 0)
``````

### Connection string matches (1)

``````
Data Source=(local)\CIRCON;Initial Catalog=circon;Integrated Security=True
``````

## CirconOpcServer.dll

### String literal constants (1)

``````
Circon.OpcXmlDa.OpcServer.File = <?xml version="1.0" encoding="utf-8"?>\r\n                         <access-policy>\r\n                           <cross-domain-access>\r\n                             <policy>\r\n                     ...(truncated)
``````

### URL matches (17)

``````
http://opcfoundation.org/webservices/XMLDA/1.0/
http://opcfoundation.org/webservices/XMLDA/1.0/a
http://opcfoundation.org/webservices/XMLDA/1.0/Browse
http://opcfoundation.org/webservices/XMLDA/1.0/d
http://opcfoundation.org/webservices/XMLDA/1.0/GetProperties
http://opcfoundation.org/webservices/XMLDA/1.0/GetStatus
http://opcfoundation.org/webservices/XMLDA/1.0/h
http://opcfoundation.org/webservices/XMLDA/1.0/i
http://opcfoundation.org/webservices/XMLDA/1.0/p
http://opcfoundation.org/webservices/XMLDA/1.0/Read
http://opcfoundation.org/webservices/XMLDA/1.0/S
http://opcfoundation.org/webservices/XMLDA/1.0/Subscribe
http://opcfoundation.org/webservices/XMLDA/1.0/SubscriptionCancel
http://opcfoundation.org/webservices/XMLDA/1.0/T
http://opcfoundation.org/webservices/XMLDA/1.0/U
http://opcfoundation.org/webservices/XMLDA/1.0/Write
http://opcfoundation.org/webservices/XMLDA/1.0/X
``````

## CirconDeviceInfo.dll

### String literal constants (6)

``````
Circon.DeviceUtil.DeviceInformation.ExtensionHelper.BASIC_LIVE = LiveCopy
Circon.DeviceUtil.DeviceInformation.ExtensionHelper.BASIC_WORKING = WorkingSet
Circon.DeviceUtil.DeviceInformation.ExtensionHelper.EXT_EEPROM = EEPROM_Names
Circon.DeviceUtil.DeviceInformation.ExtensionHelper.EXT_RAM = RAM_Names
Circon.DeviceUtil.DeviceInformation.ExtensionHelper.SCHED = ScheduleNames
Circon.DeviceUtil.DeviceInformation.ExtensionHelper.SCHED_EVENT = ScheduleEventNames
``````

## AlarmView.dll

### String literal constants (2)

``````
Circon.Forms.AlarmListView.ColumnWidthsKey = ColumnWidths
Circon.Forms.AlarmViewPropertySheet.AutomaticText = Automatic
``````

## EventTranslate.dll

### String literal constants (1)

``````
Circon.EventTranslate.EventLocalTranslations.ExtensionName = Circon_Translation
``````

## CirconSiteService.exe


## CirconOpcService.exe

### String literal constants (3)

``````
Circon.OpcXmlDa.SilverlightPolicy.File = <?xml version="1.0" encoding="utf-8"?>\r\n                         <access-policy>\r\n                           <cross-domain-access>\r\n                             <policy>\r\n                     ...(truncated)
CirconOpcService.OpcService.eventLogName = Application
CirconOpcService.OpcService.eventSource = Circon Point Service
``````

### URL matches (1)

``````
http://localhost
``````

## CirconBrowser.exe


### SQL (INSERT/UPDATE/DELETE) matches (2)

``````
Update I/O PointᄀAnything଀Page ഀTahoma଀RightഀCenter─Operation failed: ᄀ - Error༁{0}_{1}ᄀ{0}[{1}]଀[{0}]䔀Updating configuration property...㔀Wrote value '{0}' to '{1}'ԁOKጀbtnCancelഀCancel㜀LnsConfigPropertyUpdat
Update Network VariableጀSOFTWARE\ᴀ\CirconBrowser耀仹漀 瘀愀氀椀搀 䔀昀昀椀挀椀攀渀琀 䈀甀椀氀搀椀渀最 䄀甀琀漀洀愀琀椀漀渀 氀椀挀攀渀猀攀 椀猀 搀攀琀攀挀琀攀搀⸀  倀氀攀愀猀攀 愀挀琀椀瘀愀琀攀 攀椀琀栀攀爀 嘀椀猀甀愀氀 䤀渀琀攀最爀愀琀漀爀 漀爀 一攀琀眀漀爀欀 䤀渀琀攀最爀愀琀漀爀⸀
``````

## CirconCatalogManager.exe


### URL matches (6)

``````
http://crl.usertrust.com/USERTrustRSACertificationAuthority.crl0v
http://ocsp.sectigo.com0
http://ocsp.usertrust.com0
http://www.circon.com/0
https://sectigo.com/CPS0C
https://sectigo.com/CPS0D
``````

### Email matches (1)

``````
admin@circon.com
``````

## CirconOpenLns.exe


## CirconWatchdog.exe

