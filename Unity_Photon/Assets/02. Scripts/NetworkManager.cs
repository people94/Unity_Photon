using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;                   //포톤네트워크 핵심기능!!!
using Photon.Realtime;              //포톤 서비스 관련(룸옵션, 디스커넥션 등)

//네트워크 매니저 : 룸(게임공간)으로 연결시켜주는 역할
//포톤네트워크 : 마스터서버 -> 로비(대기실) -> 룸(게임공간)

//MonoBehaviourPunCallbacks : 포톤서버 접속, 로비접속, 룸접속
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text infoText;           //네트워크 상태를 보여줄 텍스트
    public Button connectButton;    //룸 접속 버튼

    string gameVersion = "1";       //게임버전

    //네트워크매니저 접속하면 가장 먼저 할일?

    private void Awake()
    {
        //해상도 설정
        Screen.SetResolution(800, 600, FullScreenMode.Windowed);
    }

    // Start is called before the first frame update
    void Start()
    {
        //접속에 필요한 정보(게임버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        //마스터 서버에 접속하는 함수(이놈이 젤 중요)
        PhotonNetwork.ConnectUsingSettings();
        //접속 시도중임을 텍스트로 표시
        infoText.text = "Connection Network Server...";
        //룸(게임공간) 접속 버튼 비활성화
        connectButton.interactable = false;        
    }

    //혹시나 시작하면서 마스터 서버에 접속 실패했을 시 자동 실행된다
    public override void OnDisconnected(DisconnectCause cause)
    {
        //룸(게임공간) 접속 버튼 비활성화
        connectButton.interactable = false;

        //접속 정보 표시하기
        infoText.text = "오프라인 : 마스터 서버와 연결실패 \n 접속을 다시 시도합니다.";

        // 마스터 서버에 접속하는 함수(이놈이 젤 중요)
        PhotonNetwork.ConnectUsingSettings();
    }

    //마스터 서버에 접속을 성공했을시 자동 실행된다
    public override void OnConnectedToMaster()
    {
        //접속 정보 표시하기
        infoText.text = "온라인 : 마스터 서버와 연결됨";
        //룸(게임공간) 접속 버튼 비활성화
        connectButton.interactable = true;
    }

    //접속 버튼 클릭시 이 함수 발동하기
    public void OnConnect()
    {
        //중복 접속 차단하기 위해 접속버튼 비활성화
        connectButton.interactable = false;

        //마스터 서버에 접속중이냐?
        if(PhotonNetwork.IsConnected)
        {
            //룸(게임세상)으로 바로 접속 실행
            infoText.text = "랜덤방에 접속...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            //마스터 서버에 접속중이 아니라면 다시 마스터 서버에 접속 시도하기
            //접속 정보 표시하기
            infoText.text = "오프라인 : 마스터 서버와 연결실패 \n 접속을 다시 시도합니다.";
            // 마스터 서버에 접속하는 함수(이놈이 젤 중요)
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        //접속 정보 표시하기
        infoText.text = "방 참가 성공했음";
        //모든 룸 참가자들이 "GameScene"을 로드함
        PhotonNetwork.LoadLevel("GameScene");
    }

    //(빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //접속 정보 표시하기
        infoText.text = "빈 방이 없으니 새로운 방 생성중...";
        //빈방을 생성한다
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }
}
