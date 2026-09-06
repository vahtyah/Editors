using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VahTyah.Feel;
using VahTyah.Inspector;

/// <summary>
/// Demo cho VahTyah Feel — showcase nhóm Transform. Bấm [Spawn Cubes] trong Inspector để dựng một lưới cube,
/// mỗi cube gắn đúng 1 feedback (default settings) để xem/chỉnh trong Inspector.
/// Vào Play: [Space] chơi tất cả (reset trước để move-to lặp lại được), [Click chuột] chơi cube được click.
/// </summary>
public class FeelDemo : MonoBehaviour
{
    [BoxGroup("Setup")]
    public float Spacing = 2.4f;

    [BoxGroup("Setup")]
    public int Columns = 4;

    private FeelPlayer[] _players;
    private (Vector3 pos, Quaternion rot, Vector3 scale)[] _initial;
    private Camera _camera;

    // Mỗi entry: nhãn + cách tạo feedback. Target để null -> Initialize tự gán transform của chính cube.
    private static readonly (string label, Func<FeelFeedback> make)[] Entries =
    {
        ("Scale Punch",        () => new ScalePunchFeedback()),
        ("Position Punch",     () => new PositionPunchFeedback()),
        ("Rotation Punch",     () => new RotationPunchFeedback()),
        ("Scale Shake",        () => new ScaleShakeFeedback()),
        ("Position Shake",     () => new PositionShakeFeedback()),
        ("Rotation Shake",     () => new RotationShakeFeedback()),
        ("Scale Spring",       () => new ScaleSpringFeedback()),
        ("Position Spring",    () => new PositionSpringFeedback()),
        ("Rotation Spring",    () => new RotationSpringFeedback()),
        ("Scale (to)",         () => new ScaleFeedback()),
        ("Position (to)",      () => new PositionFeedback()),
        ("Rotation (to)",      () => new RotationFeedback()),
        ("Squash & Stretch",   () => new SquashAndStretchFeedback()),
        ("Squash & Stretch Spring", () => new SquashAndStretchSpringFeedback()),
        ("Rotate Pos Around",  () => new RotatePositionAroundFeedback()),
        ("Wiggle",             () => new WiggleFeedback()),
    };

    // --- Editor buttons ------------------------------------------------------

    [Button("Spawn Cubes")]
    public void SpawnCubes()
    {
        ClearCubes();

        int total = Entries.Length;
        int cols = Mathf.Max(1, Columns);
        int rows = Mathf.CeilToInt(total / (float)cols);

        for (int i = 0; i < total; i++)
        {
            int col = i % cols;
            int row = i / cols;
            Vector3 pos = new Vector3(
                (col - (cols - 1) * 0.5f) * Spacing,
                ((rows - 1) * 0.5f - row) * Spacing,
                0f);

            FeelPlayer player = CreateCube(Entries[i].label, pos, out Transform t);
            FeelFeedback fb = Entries[i].make();
            fb.Label = Entries[i].label;
            player.AddFeedback(fb);
            MarkDirty(player);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
        Debug.Log($"[FeelDemo] Đã spawn {total} cube (mỗi cube 1 feedback nhóm Transform). Play rồi [Space]/[Click].");
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
        int total = Entries.Length;
        int cols = Mathf.Max(1, Columns);
        int rows = Mathf.CeilToInt(total / (float)cols);

        _camera = Camera.main;
        if (_camera != null)
        {
            float dist = Mathf.Max(cols, rows) * Spacing * 1.3f + 4f;
            _camera.transform.position = new Vector3(0f, 0f, -dist);
            _camera.transform.rotation = Quaternion.identity;
        }

        _players = GetComponentsInChildren<FeelPlayer>();
        _initial = new (Vector3, Quaternion, Vector3)[_players.Length];
        for (int i = 0; i < _players.Length; i++)
        {
            Transform t = _players[i].transform;
            _initial[i] = (t.localPosition, t.localRotation, t.localScale);
        }

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
                ResetCube(i);
                _players[i].Play();
            }
        }

        if (Input.GetMouseButtonDown(0) && _camera != null)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) &&
                hit.collider.TryGetComponent(out FeelPlayer player))
            {
                int idx = Array.IndexOf(_players, player);
                if (idx >= 0)
                {
                    ResetCube(idx);
                }
                player.PlayAsync(new FeelPlayContext(hit.point)).Forget();
            }
        }
    }

    // Trả cube về TRS gốc trước khi chơi lại (để các feedback move-to/rotate-to lặp lại được).
    private void ResetCube(int i)
    {
        if (_players[i] == null)
        {
            return;
        }
        Transform t = _players[i].transform;
        t.localPosition = _initial[i].pos;
        t.localRotation = _initial[i].rot;
        t.localScale = _initial[i].scale;
    }

    // --- Helpers -------------------------------------------------------------

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
        GUI.Label(new Rect(10, 10, 720, 20),
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
            Vector3 sp = _camera.WorldToScreenPoint(_players[i].transform.position + Vector3.up * 0.75f);
            if (sp.z <= 0f)
            {
                continue;
            }
            GUI.Label(new Rect(sp.x - 90f, Screen.height - sp.y, 180f, 20f), _players[i].name);
        }
    }
}
