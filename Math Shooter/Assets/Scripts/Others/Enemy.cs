
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public TextMeshPro numberLabel;
    public int myNumber;
    public float speed = 1.5f;

    private float limitaY = -3.5f;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
        if (transform.position.y <= limitaY)
        {
            float novoX = Random.Range(-6f, 6f);
            transform.position = new Vector3(novoX, 4f, 0);
        }
    }
    void Start()
    {
        // Garantir que o inimigo apareça
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = 15;
            sr.sortingLayerName = "Default";
        }

        // Garantir que o TextMeshPro também apareça
        if (numberLabel != null)
        {
            numberLabel.sortingOrder = 16;
        }
    }

    public void SetNumber(int number)
    {
        myNumber = number;
        numberLabel.text = number.ToString();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Bullet")) return;

        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet == null || !bullet.TentarAcertar()) return;

        Destroy(other.gameObject);

        // ✅ Checa a resposta PRIMEIRO para saber se vai ser game over
        bool eraCorreto = (myNumber == GameManager.instance.GetCorrectAnswer());

        // Efeito visual de explosão sempre acontece
        if (EfeitosManager.instance != null)
            EfeitosManager.instance.ExplodirInimigo(transform.position);

        // ✅ Som de explosão só toca se for acerto
        //    No erro, o GameManager cuida do som (game over tem animação própria)
        if (eraCorreto && SoundManager.instance != null)
            SoundManager.instance.TocarExplosao();

        GameManager.instance.CheckAnswer(myNumber);
        Destroy(gameObject);
    }
}
