<?php

class Controller_Home extends Controller_Rest
{
	/**
	 * The 404 action for the application.
	 *
	 * @access  public
	 * @return  Response
	 */
	public function action_404() {
		return NotFound::response404();
	}

	public function get_ranking() {
		// ランキングをJSONで返す。最新N件
		return $this->response(Model_Ranking::get());
	}

	private function create_player_id() {
		while (Model_Player::exist($id = Str::random('alnum', 100))) ;
		return $id;
	}

	public function post_rank_entry() {
		// 記録を登録する
		$id = Input::post('id');
		$name = Input::post('name');
		$score = Input::post('score');

		if (!Model_Player::exist($id)) {
			Model_Player::add($id, $name);
		}

		$prev_score = Model_Ranking::get_score($id);
		if ($score < $prev_score) {
			return $this->response(array(
				'is_new_record' => intval($score) > intval($prev_score),
				'score' => $score,
				'prev' => $prev_score
			));
		}

		Model_Player::update_name($id, $name);
		Model_Ranking::entry($id, $score);

		return $this->response(array(
			'is_new_record' => intval($score) > intval($prev_score),
			'score' => $score,
			'prev' => $prev_score
		));
	}

	public function post_rank_first_entry() {
		// 記録を登録する
		$id = self::create_player_id();
		$name = Input::post('name');
		$score = Input::post('score');

		Model_Player::add($id, $name);

		$prev_score = Model_Ranking::get_score($id);
		Model_Ranking::entry($id, $score);

		return $this->response(array(
			'is_new_record' => intval($score) > intval($prev_score),
			'score' => $score,
			'prev' => $prev_score,
			'id' => $id,
			'name' > $name
		));
	}
}
