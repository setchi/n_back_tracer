
CREATE TABLE n_back_tracer_player (
	id varchar(100) collate utf8_unicode_ci default NULL,
	name varchar(10),
	PRIMARY KEY (id)
) ENGINE=InnoDB;


CREATE TABLE n_back_tracer_ranking (
	player_id varchar(100) collate utf8_unicode_ci default NULL,
	chain_n varchar (5),
	score int default NULL,
	time datetime default '0000-00-00 00:00:00',
	PRIMARY KEY (player_id, time),
	FOREIGN KEY (player_id)
		REFERENCES n_back_tracer_player (id)
) ENGINE=InnoDB;
