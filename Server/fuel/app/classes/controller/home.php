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
		return $this->response(Model_Ranking::get(100));
	}

	public function get_create_player_id() {
		$default_name  = "guest";

		while (Model_Player::exist($id = Str::random('alnum', 100))) ;
		Model_Player::add($id, $default_name);

		return $this->response(array(
			'id' => $id,
			'name' => $default_name
		));
	}

	public function post_regist_if_new_record() {
		// 記録を登録する
		$id = Input::post('id');
		$chain_n = Input::post('chainAndN');
		$score = Input::post('score');

		$prev_score = Model_Ranking::get_score($id);
		if ($score < $prev_score) {
			return $this->response(array('is_new_record' => false));
		}

		Model_Ranking::entry($id, $chain_n, $score);

		return $this->response(array('is_new_record' => true));
	}

	public function post_update_player_name() {
		$id = Input::post("id");
		$name = Input::post("name");

		Model_Player::update_name($id, $name);

		return $this->response(array());
	}
}
