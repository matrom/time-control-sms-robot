insert into Users(FirstName, Surname, login, passwd, deadline)
values ('', 'Bank', null, null, cast(1000000 as datetime)),
('John', 'Dou', null, null, cast(45000 as datetime)),
('Abraham', 'Adners', 'aanders', 'qwerty', dateadd(hh, 2, GETDATE())),
('Abraham', 'Adners', 'aanders', 'qwerty', dateadd(hh, 2, GETDATE())),
('Bob', 'Baker', 'bbaker', 'qwerty', dateadd(hh, 2, GETDATE())),
('Casey', 'Calahan', 'ccalahan', 'qwerty', dateadd(hh, 2, GETDATE())),
('Don', 'Digglz', 'ddigglz', 'qwerty', dateadd(hh, 2, GETDATE())),
('Effie', 'Enwood', 'eenwood', 'qwerty', dateadd(hh, 2, GETDATE())),
('Frank', 'Foster', 'ffoster', 'qwerty', dateadd(hh, 2, GETDATE())),
('Govard', 'Gover', 'ggover', 'qwerty', dateadd(hh, 2, GETDATE())),
('Helen', 'Hansen', 'hhansen', 'qwerty', dateadd(hh, 2, GETDATE()))