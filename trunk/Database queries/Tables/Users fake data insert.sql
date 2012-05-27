--truncate table [Messages]
--truncate table [PhoneNumbers]
--delete from Users

insert into Users(FirstName, Surname, login, passwd, deadline, ServiceNotes)
values ('', 'Bank', null, null, cast('12-06-1748' as datetime2), 'Bank'),
('John', 'Dou', null, null, cast(45000 as datetime), null),
('Abraham', 'Adners', 'aanders', 'qwerty', dateadd(hh, 2, GETDATE()), null),
('Abraham', 'Adners', 'aanders', 'qwerty', dateadd(hh, 2, GETDATE()), null),
('Bob', 'Baker', 'bbaker', 'qwerty', dateadd(hh, 2, GETDATE()), null),
('Casey', 'Calahan', 'ccalahan', 'qwerty', dateadd(hh, 2, GETDATE()), null),
('Don', 'Digglz', 'ddigglz', 'qwerty', dateadd(hh, 2, GETDATE()), null),
('Effie', 'Enwood', 'eenwood', 'qwerty', dateadd(hh, 2, GETDATE()), null),
('Frank', 'Foster', 'ffoster', 'qwerty', dateadd(hh, 2, GETDATE()), null),
('Govard', 'Gover', 'ggover', 'qwerty', dateadd(hh, 2, GETDATE()), null),
('Helen', 'Hansen', 'hhansen', 'qwerty', dateadd(hh, 2, GETDATE()), null)
