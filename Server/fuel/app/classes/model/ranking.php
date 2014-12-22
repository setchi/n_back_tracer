<?php

class Model_Ranking extends Model
{
	private static $prefix = 'n_back_tracer_';

	public static function exist($id) {
		$exist = DB::select()->from(self::$prefix.'ranking')
			->where('player_id', $id)
			->execute()->count();

		return $exist != 0;
	}

	public static function entry($id, $score) {
		if (self::exist($id)) {
			DB::update(self::$prefix.'ranking')->set(array(
				'score' => $score,
				'time' => date('Y-m-d H:i:s')
			))->where('player_id', $id)->execute();

		} else {
			DB::insert(self::$prefix.'ranking')->set(array(
				'player_id' => $id,
				'score' => $score,
				'time' => date('Y-m-d H:i:s')
			))->execute();
		}
	}

	public static function get_score($id) {
		if (!self::exist($id))
			return 0;

		$record = DB::select('score')->from(self::$prefix.'ranking')
			->where('player_id', $id)
			->execute()->as_array();

		return $record[0]['score'];
	}

	public static function get() {
		return DB::select(self::$prefix.'player.name', 'score')->from(self::$prefix.'ranking')
			->join(self::$prefix.'player')
			->on(self::$prefix.'ranking.player_id', '=', self::$prefix.'player.id')
			->order_by('score', 'desc')
			->limit(10)
			->execute()->as_array();
	}
}
