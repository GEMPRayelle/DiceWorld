using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class pawn_move : MonoBehaviour
{
    public int player_ID; // 플레이어 식별자
    public Vector3 targetPosition; // 이동할 목표 위치
    public int move_point; // 이동할 포인트
    public int current_index; // 현재 타일 인덱스
    public int current_row; // 현재 행
    public GameObject gameBoard; // 게임 보드 오브젝트
    public GameObject gameManager; // 게임 매니저 오브젝트
    public float movementSpeed = 2.0f; // 이동 속도
    public int maxTile_num; // 최대 타일 개수
    bool isMoved = false; // 이동 여부 플래그
    public float next_turn_count = 0;
    bool is_not_start = false;
    void Start()
    {
        gameManager = GameObject.Find("game_manager");
        gameBoard = GameObject.Find("boardCenter");
    }

    void Update()
    {
        maxTile_num = gameManager.GetComponent<tile_manager>().max_tile_num;
        current_row = current_index / 8;

        if (current_index >= 0)
        {
            current_index = current_index % (maxTile_num + 1);
        }
        else
        {
            current_index = (maxTile_num + 1) + current_index;
        }

        // 다음 타일의 Transform을 찾아 목표 위치 설정
        Transform now_tile_trans = gameBoard.transform.Find("tile_" + current_index);
        targetPosition = new Vector3(now_tile_trans.position.x, transform.position.y, now_tile_trans.position.z);

        // 현재 위치에서 목표 위치로 보간하여 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.0001f)
        {
            transform.position = targetPosition;
        }

        // 출발시점+이동 포인트가 남아 있고 목표 위치에 도달했을 때
        if (move_point > 0 && transform.position == targetPosition)
        {

            isMoved = true;
            current_index += 1;
            move_point -= 1;
            string event_tag = now_tile_trans.gameObject.GetComponent<tile_event_manager>().event_tag;
            if (event_tag != "nothing" && is_not_start == true)
            {
                now_tile_trans.gameObject.GetComponent<tile_event_manager>().passing_event();
            }
            else
            {
                ;
            }
            is_not_start = true;
        }

        // 이동이 완료되었고 이동 포인트가 없을 때
        if (isMoved && move_point <= 0 && (Vector3.Distance(transform.position, targetPosition) <= 0.01f))
        {
            transform.position = targetPosition;
            // 현재 플레이어의 차례인 경우
            if (gameManager.GetComponent<game_manager>().currentPlayerIndex == player_ID)
            {
                is_not_start = false;
                next_turn_count += Time.deltaTime;
                if (next_turn_count >= 0.3f)
                {
                    next_turn_count = 0f;
                    isMoved = false;
                    string event_tag = now_tile_trans.gameObject.GetComponent<tile_event_manager>().event_tag;

                    // 특정 위치에 다른 플레이어가 있는지 확인
                    int otherPlayerID = GetOtherPlayerIDAtLocation(current_index);
                    if (otherPlayerID != -1)
                    {
                        Debug.Log(player_ID + "/" + otherPlayerID);
                        // 플레이어 간 싸움
                        now_tile_trans.gameObject.GetComponent<tile_event_manager>().Fight(player_ID, otherPlayerID);
                    }
                    else if (event_tag != "nothing")
                    {
                        now_tile_trans.gameObject.GetComponent<tile_event_manager>().active_event();
                    }
                    else
                    {
                        gameManager.GetComponent<game_manager>().nextTurn(); // 다음 턴 진행
                    }
                }
            }
        }
    }

    int GetOtherPlayerIDAtLocation(int currentLocation)
    {
        foreach (var player in gameManager.GetComponent<game_manager>().players)
        {
            if(player != null){
                pawn_move playerPawn = player.GetComponent<pawn_move>();
                if (playerPawn.current_index == currentLocation && playerPawn.player_ID != player_ID && playerPawn != null)
                {
                    return playerPawn.player_ID;
                }
            }else{
                continue;
            }
        }
        return -1; // 해당 위치에 다른 플레이어가 없음
    }

    // 주사위 결과에 따라 폰을 이동시키는 메서드
    public void move_pawn(int diceResult)
    {
        move_point += diceResult;
    }
}

