using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class HiddenPlayerCheck
{
    private const string Done = "HiddenPlayerCheckDoneV2";
    private static readonly StringBuilder report = new StringBuilder();

    static HiddenPlayerCheck() { EditorApplication.delayCall += Run; }

    private static void Set(object target, string name, object value)
    {
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    }

    private static object Get(object target, string name) => target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);

    private static GameObject Child(GameObject root, string name)
    {
        var child = new GameObject(name);
        child.transform.SetParent(root.transform, false);
        return child;
    }

    private static cuadroscript Tile(GameObject root, string name, Vector3 exitPosition)
    {
        var tileObject = Child(root, name);
        var tile = tileObject.AddComponent<cuadroscript>();
        var exit = Child(tileObject, "Exit");
        exit.transform.position = exitPosition;
        Set(tile, "puntoSalida", exit.transform);
        Set(tile, "aviso", Child(tileObject, "Prompt"));
        Set(tile, "eventoCuadro", Child(tileObject, "Event"));
        return tile;
    }

    private static void Require(bool condition, string description)
    {
        if (!condition) throw new InvalidOperationException(description);
        report.AppendLine("PASS: " + description);
    }

    private static void Run()
    {
        if (SessionState.GetBool(Done, false) || EditorApplication.isCompiling || SceneManager.GetActiveScene().name != "ikerproyectada") return;
        if (!EditorApplication.isPlaying)
        {
            SessionState.SetBool("HiddenPlayerCheckExitPlay", true);
            EditorApplication.EnterPlaymode();
            return;
        }
        SessionState.SetBool(Done, true);
        GameObject fixture = null;
        try
        {
            var reference = UnityEngine.Object.FindFirstObjectByType<IAZone>();
            Require(reference != null, "An enemy with patrol zones is available.");
            var referenceAgent = reference.GetComponent<NavMeshAgent>();
            Require(NavMesh.SamplePosition(reference.transform.position, out var spawn, 1, NavMesh.AllAreas), "Test agent can use the saved house NavMesh.");
            fixture = new GameObject("__HiddenPlayerCheck");
            fixture.SetActive(false);
            var playerObject = Child(fixture, "Player");
            playerObject.layer = 31;
            playerObject.transform.position = spawn.position + Vector3.up * 0.5f;
            playerObject.AddComponent<SpriteRenderer>();
            playerObject.AddComponent<BoxCollider2D>().isTrigger = true;
            playerObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            playerObject.AddComponent<movementpj>().enabled = false;
            var player = playerObject.AddComponent<Playerinteract>();
            player.enabled = false;
            var counterObject = new GameObject("Counter", typeof(RectTransform));
            counterObject.transform.SetParent(fixture.transform, false);
            var counter = counterObject.AddComponent<TMPro.TextMeshProUGUI>();
            counter.enabled = false;
            Set(player, "textoAjolotes", counter);
            var first = Tile(fixture, "First Tile", playerObject.transform.position);
            var second = Tile(fixture, "Second Tile", spawn.position + Vector3.right * 0.5f);
            Set(player, "cuadroActual", first);
            var enemy = Child(fixture, "Enemy");
            enemy.transform.position = spawn.position;
            var agent = enemy.AddComponent<NavMeshAgent>();
            agent.agentTypeID = referenceAgent.agentTypeID;
            agent.radius = referenceAgent.radius;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            var ai = enemy.AddComponent<IAZone>();
            ai.enabled = false;
            ai.zonas = reference.zonas;
            ai.transformJugador = playerObject.transform;
            ai.capaJugador = 1 << 31;
            ai.velocidadPatrulla = 2;
            ai.velocidadPersecucion = 6;
            fixture.SetActive(true);
            Require(agent.Warp(spawn.position), "Test agent is on the NavMesh.");
            Physics2D.SyncTransforms();
            Require(Physics2D.OverlapCircle(agent.transform.position, ai.radioDeteccion, ai.capaJugador) != null, "Fixture player is present in physics detection queries.");
            ai.SendMessage("Start");
            ai.SendMessage("Update");
            Require((EstadoIA)Get(ai, "estadoActual") == EstadoIA.Persiguiendo, "Visible player is pursued.");
            int previousZone = (int)Get(ai, "zonaActualIndex");
            player.SendMessage("Interactuar");
            Require(player.EstaDentroDelCuadro(), "Player enters a tile through the normal interaction method.");
            ai.SendMessage("Update");
            Require((EstadoIA)Get(ai, "estadoActual") == EstadoIA.Patrullando, "Hiding immediately interrupts pursuit.");
            Require(ai.transformJugador == null, "The enemy forgets the player Transform.");
            Require(Mathf.Approximately(agent.speed, ai.velocidadPatrulla), "Patrol speed is restored.");
            Require((int)Get(ai, "zonaActualIndex") != previousZone, "Another patrol zone is selected.");
            var zone = ai.zonas[(int)Get(ai, "zonaActualIndex")];
            var destination = zone.puntosDeZona[(int)Get(ai, "puntoActualIndex")].position;
            Require(Vector3.Distance(agent.destination, destination) < 0.1f, "The pursuit destination is replaced by a patrol waypoint.");
            for (int i = 0; i < 8; i++) ai.SendMessage("Update");
            Require(ai.transformJugador == null && (EstadoIA)Get(ai, "estadoActual") == EstadoIA.Patrullando, "Hidden player is not reacquired despite remaining inside detection range.");
            player.TeletransportarACuadro(second);
            Physics2D.SyncTransforms();
            ai.SendMessage("Update");
            Require(ai.transformJugador == null && player.EstaDentroDelCuadro(), "Teleporting between tiles keeps the player undetectable.");
            player.SendMessage("Interactuar");
            ai.SendMessage("Update");
            Require((EstadoIA)Get(ai, "estadoActual") == EstadoIA.Persiguiendo && ai.transformJugador == player.transform, "Player can be detected again after leaving the tile.");
        }
        catch (Exception error) { report.AppendLine("FAIL: " + error); }
        finally
        {
            if (fixture != null) UnityEngine.Object.DestroyImmediate(fixture);
            Directory.CreateDirectory("Logs/HiddenPlayerCheck");
            File.WriteAllText("Logs/HiddenPlayerCheck/Report.txt", report.ToString());
            if (SessionState.GetBool("HiddenPlayerCheckExitPlay", false)) EditorApplication.ExitPlaymode();
        }
    }
}
