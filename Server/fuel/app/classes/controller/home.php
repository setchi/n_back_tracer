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

	public function get_create_player_id() {
		// 新しいuser_idを返す
		
		while (Model_Player::exist($id = Str::random('alnum', 100))) ;

		Model_Player::add($id, "");

		return $this->response(array(
			'id' => $id,
			'name' => ""
		));
	}

	public function post_rank_entry() {
		// 記録を登録する
		$id = Input::post('id');
		$name = Input::post('name');
		$score = Input::post('score');

		if (!Model_Player::exist($id)) {
			Model_Player::add($id, $name);
		}
		Model_Player::update_name($id, $name);

		Model_Ranking::entry($id, $score);

		return $this->response(array('entry' => true));
	}

	public function get_check_record() {
		return $this->response(array(
			'is_new_record' => true
		));


		$prev_score = Model_Ranking::get_score($id);

		return $this->response(array(
			'is_new_record' => $score > $prev_score
		));
	}
}
