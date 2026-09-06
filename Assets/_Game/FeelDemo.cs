using Cysharp.Threading.Tasks;
using UnityEngine;
using VahTyah.Feel;
using VahTyah.Inspector;

/// <summary>
/// Demo cho VahTyah Feel. Bấm nút [Spawn Cubes] trong Inspector để dựng 3 cube thật (edit mode)
/// — mỗi cube có sẵn FeelPlayer + feedback để bạn chỉnh trực tiếp trong Inspector.
/// Vào Play: [Space] chơi tất cả, [Click chuột] chơi cube được click.
/// </summary>
public class FeelDemo : MonoBehaviour
{
    [BoxGroup("Setup")]
    public float Spacing = 2.2f;

    private FeelPlayer[] _players;
    private Camera _camera;

    // --- Editor buttons ------------------------------------------------------

    [Button("Spawn Cubes")]
    public void SpawnCubes()
    {
        ClearCubes();

        BuildSinglePunch("Cube - Single Punch", new Vector3(-Spacing, 0f, 0f));
        BuildSequential("Cube - Sequential (AfterPrevious)", Vector3.zero);
        BuildParallel("Cube - Parallel (WithPrevious)", new Vector3(Spacing, 0f, 0f));

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
        Debug.Log("[FeelDemo] Đã spawn 3 cube. Chỉnh feel trong Inspector từng cube, rồi Play để xem.");
    }

    [Button("Clear Cubes")]
    public void ClearCubes()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
    }

    // --- Play mode -----------------------------------------------------------

    private void Start()
    {
        _camera = Camera.main;
        if (_camera != null)
        {
            _camera.transform.position = new Vector3(0f, 1.5f, -7f);
            _camera.transform.rotation = Quaternion.Euler(8f, 0f, 0f);
        }

        _players = GetComponentsInChildren<FeelPlayer>();
        Debug.Log("[FeelDemo] Space = chơi tất cả | Click chuột = chơi cube đó");
    }

    private void Update()
    {
        if (_players == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < _players.Length; i++)
            {
                _players[i].Play();
            }
        }

        if (Input.GetMouseButtonDown(0) && _camera != null)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) &&
                hit.collider.TryGetComponent(out FeelPlayer player))
            {
                player.PlayAsync(new FeelPlayContext(hit.point)).Forget();
            }
        }
    }

    // --- Builders ------------------------------------------------------------

    private void BuildSinglePunch(string name, Vector3 pos)
    {
        FeelPlayer player = CreateCube(name, pos, out Transform t);
        player.AddFeedback(new ScalePunchFeedback
        {
            Label = "Punch",
            Target = t,
            Strength = Vector3.one * 0.4f,
            TweenDuration = 0.4f
        });
        MarkDirty(player);
    }

    private void BuildSequential(string name, Vector3 pos)
    {
        FeelPlayer player = CreateCube(name, pos, out Transform t);
        // Punch dọc trước, xong mới punch ngang -> tổng thời gian = tổng 2 cái.
        player.AddFeedback(new ScalePunchFeedback
        {
            Label = "Punch Y",
            Target = t,
            TimingMode = FeelTimingMode.AfterPrevious,
            Strength = new Vector3(0f, 0.6f, 0f),
            TweenDuration = 0.35f
        });
        player.AddFeedback(new ScalePunchFeedback
        {
            Label = "Punch X",
            Target = t,
            TimingMode = FeelTimingMode.AfterPrevious,
            Strength = new Vector3(0.6f, 0f, 0f),
            TweenDuration = 0.35f
        });
        MarkDirty(player);
    }

    private void BuildParallel(string name, Vector3 pos)
    {
        FeelPlayer player = CreateCube(name, pos, out Transform t);
        // Scale + Position punch chạy cùng lúc (WithPrevious): cube vừa phồng vừa nảy lên.
        // Hai feedback ghi property khác nhau (scale vs position) nên không tranh chấp.
        player.AddFeedback(new ScalePunchFeedback
        {
            Label = "Scale Punch",
            Target = t,
            TimingMode = FeelTimingMode.AfterPrevious,
            Strength = Vector3.one * 0.35f,
            TweenDuration = 0.45f
        });
        player.AddFeedback(new PositionPunchFeedback
        {
            Label = "Position Punch",
            Target = t,
            TimingMode = FeelTimingMode.WithPrevious,
            Strength = new Vector3(0f, 0.6f, 0f),
            TweenDuration = 0.45f
        });
        MarkDirty(player);
    }

    private FeelPlayer CreateCube(string name, Vector3 localPos, out Transform cubeTransform)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(transform);
        cube.transform.localPosition = localPos;
        cubeTransform = cube.transform;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.Undo.RegisterCreatedObjectUndo(cube, "Spawn Feel Cube");
        }
#endif
        return cube.AddComponent<FeelPlayer>();
    }

    private static void MarkDirty(FeelPlayer player)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(player);
        }
#endif
    }

    // --- On-screen help ------------------------------------------------------

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 640, 20),
            "VahTyah Feel Demo — [Space] chơi tất cả, [Click] chơi cube được click");

        if (_camera == null || _players == null)
        {
            return;
        }

        for (int i = 0; i < _players.Length; i++)
        {
            if (_players[i] == null)
            {
                continue;
            }
            Vector3 sp = _camera.WorldToScreenPoint(_players[i].transform.position + Vector3.up * 0.9f);
            if (sp.z <= 0f)
            {
                continue;
            }
            GUI.Label(new Rect(sp.x - 110f, Screen.height - sp.y, 220f, 20f), _players[i].name);
        }
    }
}
