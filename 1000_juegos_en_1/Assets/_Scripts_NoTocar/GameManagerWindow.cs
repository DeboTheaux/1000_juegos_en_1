using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Types;

public class GameManagerWindow : EditorWindow
{

    static Texture2D logoWIG;
    Grid gridPrefab;

    [MenuItem("Window/GameManager")]
    public static void ShowWindow()
    {
        GetWindow<GameManagerWindow>("Game Manager");
    }

    private void OnEnable()
    {
        gridPrefab = Resources.Load<Grid>("Prefabs/Grid");
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(100, 10, 300, 100));

        Header();

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(15, 50, 300, 500));

        Options();

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(30, 210, 270, 500));

        if (GUILayout.Button("Create"))
        {
            CreateScene();
        }

        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(30, 500, 700, 500));

        Sign();

        GUILayout.EndArea();
    }

    void Header()
    {
        GUILayout.Label("OPCIONES DEL JUEGO", EditorStyles.whiteBoldLabel);
    }

    void Options()
    {

        GameInfo.nombreEscena = EditorGUILayout.TextField("Nombre de tu juego: ", GameInfo.nombreEscena);

        GameInfo.tipoDeVista = (TipoDeVista)EditorGUILayout.EnumPopup("Vista de la Cámara: ", GameInfo.tipoDeVista);

        GameInfo.tipoDeControl = (TipoDeControl)EditorGUILayout.EnumPopup("Control del Player: ", GameInfo.tipoDeControl);


        GameInfo.tipoDeMovimiento = (TipoDeMovimiento)EditorGUILayout.EnumPopup("Movimiento del Player: ", GameInfo.tipoDeMovimiento);

        GUILayout.Label("Opciones adicionales: ", EditorStyles.boldLabel);

        GameInfo.camaraPersigue = EditorGUILayout.Toggle("Seguimiento de Cámara: ", GameInfo.camaraPersigue);

        if (GameInfo.camaraPersigue)
        {
            GameInfo.demora = EditorGUILayout.Slider("Demora", GameInfo.demora, 0, 0.5f);
        }

        if (GameInfo.tipoDeMovimiento == TipoDeMovimiento.DosDirecciones)
        {
            GameInfo.puedeSaltar = EditorGUILayout.Toggle("Puede saltar: ", GameInfo.puedeSaltar);
        }
    }

    void Sign()
    {
        logoWIG = Resources.Load<Texture2D>("WIG/WIG-Logo");
        EditorGUI.DrawTextureTransparent(new Rect(0, 0, 256, 117), logoWIG);

        GUILayout.Space(110);

        GUILayout.Label("\n Herramienta desarrollada \n por Womens In Games Argentina", EditorStyles.whiteBoldLabel);

        if (GUILayout.Button("http://www.womeningamesar.com/", EditorStyles.linkLabel))
            Application.OpenURL("http://www.womeningamesar.com/");
    }

    static Scene escena;

    void CreateScene()
    {
        escena = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        escena.name = GameInfo.nombreEscena;

        EditorSceneManager.SaveScene(escena, "Assets/Scenes/" + GameInfo.nombreEscena + ".unity");

        Debug.LogFormat("¡Felicitaciones! Creaste tu plantilla. ".Bold().Size(13) + "Para pintar el mapa, no te olvides de abrir el panel " + "Window/2D/Tile Palette".Color("red").Bold());

        if (GameInfo.camaraPersigue)
        {
            CameraFollow follow = Camera.main.gameObject.AddComponent<CameraFollow>();
            follow.velocidad = GameInfo.demora;
        }

        Grid clon = Instantiate(gridPrefab, Vector2.zero, Quaternion.identity);

        clon.name = "Mapa";

        switch (GameInfo.tipoDeVista)
        {
            //Cargar por default el TilePalette
            //Agregar los Tiles a la paleta > Consultar con Steph

            case TipoDeVista.Isometric:
                clon.cellLayout = GridLayout.CellLayout.IsometricZAsY;
                clon.cellSize = new Vector3(1, 0.5f, 1);
                clon.GetComponentInChildren<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopRight;
                break;
            case TipoDeVista.SideScroller:
                clon.cellLayout = GridLayout.CellLayout.Rectangle;
                clon.cellSize = new Vector3(1, 1, 1);
                clon.GetComponentInChildren<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.BottomLeft;
                break;
            case TipoDeVista.TopDown:
                clon.cellLayout = GridLayout.CellLayout.Rectangle;
                clon.cellSize = new Vector3(1, 1, 1);
                clon.GetComponentInChildren<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopRight;
                break;
        }

        PlayerController player = new GameObject("Player").AddComponent<PlayerController>();
        GameObject checkFloor = new GameObject("CheckFloor");

        checkFloor.transform.SetParent(player.transform);

        SpriteRenderer sr = player.gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(logoWIG, new Rect(10, 10, 250, 305), Vector2.zero);
        player.gameObject.AddComponent<BoxCollider2D>();
        Rigidbody2D rb = player.gameObject.AddComponent<Rigidbody2D>();
        rb.drag = 1;
        rb.angularDrag = 1;
        rb.freezeRotation = true;

        Debug.LogFormat("¡Selecciona al Objeto Player para cambiarle el Sprite!".Bold().Size(13).Color("blue"));

        switch (GameInfo.tipoDeMovimiento)
        {
            case TipoDeMovimiento.DosDirecciones:
                rb.gravityScale = 1;
                break;
            case TipoDeMovimiento.CuatroDirecciones:
                rb.gravityScale = 0;
                break;
            case TipoDeMovimiento.OchoDirecciones:
                rb.gravityScale = 0;
                break;
            default:
                break;
        }

        if (GameInfo.puedeSaltar)
        {
            player.puedeSaltar = true;
        }

        switch (GameInfo.tipoDeControl)
        {
            //CODEAR PARA QUE SE AGREGUEN LOS EJES
            case TipoDeControl.Flechitas:
                player.horizontalAxis = "Horizontal2";
                player.verticalAxis = "Vertical2";
                break;
            case TipoDeControl.WASD:
                player.horizontalAxis = "Horizontal";
                player.verticalAxis = "Vertical";
                break;
            case TipoDeControl.PointAndClick:
                player.horizontalAxis = "Mouse X";
                player.verticalAxis = "Mouse Y";
                player.pointAndClick = true;
                break;
        }

        player.tipoDeMovimiento = GameInfo.tipoDeMovimiento;
        Close();
    }
}

namespace Types
{
    public enum TipoDeVista
    {
        TopDown,
        Isometric,
        SideScroller
    }

    public enum TipoDeControl
    {
        WASD,
        Flechitas,
        PointAndClick
    }

    public enum TipoDeMovimiento
    {
        DosDirecciones,
        CuatroDirecciones,
        OchoDirecciones
    }
}


