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

CREATE TABLE spider_content_xxx
(
    Id               int AUTO_INCREMENT
        PRIMARY KEY,
    ExtraProperties  longtext    NULL,
    ConcurrencyStamp varchar(40) NULL,
    已采             int         NOT NULL,
    已发             int         NOT NULL,
    标题             longtext    NOT NULL,
    内容             longtext    NOT NULL,
    作者             longtext    NULL,
    时间             datetime(6) NOT NULL,
    出处             longtext    NOT NULL,
    PageUrl          longtext    NULL,
    关键字           longtext    NULL,
    tag              longtext    NULL,
    GroupId          longtext    NULL
);

INSERT INTO spider_content_xxx
SELECT arch.id       AS Id,
       ''            AS ExtraProperties,
       ''            AS ConcurrencyStamp,
       1             AS 已采,
       0             AS 已发,
       arch.title    AS 标题,
       da.body       AS 内容,
       ''            AS 作者,
       NOW()         AS 时间,
       ''            AS 出处,
       ''            AS PageUrl,
       arch.keywords AS 关键字,
       ''            AS tag,
       ''            AS GroupId
FROM dede_archives AS arch
         INNER JOIN dede_addonarticle da ON arch.typeid = da.typeid AND arch.id = da.aid
ORDER BY arch.id ASC;