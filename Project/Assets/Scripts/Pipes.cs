using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipes : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        // 管道的中间位置的探测区域，如果碰撞物体为小鸟（被标记为了Player），计分
        if (collision.tag == "Player") {
            Debug.Log("You are good!");

            GameController.instance.PassOnePip();
            SoundManager.instance.PlayPass();
        }
    }
}
