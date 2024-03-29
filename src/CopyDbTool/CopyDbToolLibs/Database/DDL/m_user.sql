CREATE TABLE m_user_src(

  user_id varchar(36) NOT NULL,
  user_name varchar(256) NOT NULL,
  disp_name nvarchar(256) NOT NULL,
  del_flg bit default 0 NOT NULL,
  s_ins_ymd datetime NOT NULL,
  s_ins_usr varchar(36) NOT NULL,
  s_upd_ymd datetime,
  s_upd_usr varchar(36),

  CONSTRAINT PK_m_user_src PRIMARY KEY (user_id)
);
CREATE TABLE m_user_dst(

  user_id varchar(36) NOT NULL,
  user_name varchar(256) NOT NULL,
  disp_name nvarchar(256) NOT NULL,
  del_flg bit default 0 NOT NULL,
  imp_flg bit default 0 NOT NULL,
  s_ins_ymd datetime NOT NULL,
  s_ins_usr varchar(36) NOT NULL,
  s_upd_ymd datetime,
  s_upd_usr varchar(36),

  CONSTRAINT PK_m_user_dst PRIMARY KEY (user_id)
);
