select arch.id       as Id,
       1             as 已采,
       0             as 已发,
       arch.title    as 标题,
       da.body       as 内容,
       arch.keywords as 关键字
from dede_archives as arch
         inner join dede_addonarticle da on arch.typeid = da.typeid and arch.id = da.aid
where arch.typeid = 1
order by arch.id asc;