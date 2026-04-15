namespace HJW
{
    using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int selectStage =1;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
           Destroy(gameObject);
        }
    }

    public void SelectStage(int stageNum)
    {
        selectStage =stageNum;
        Debug.Log(stageNum);
    }    
}

}
