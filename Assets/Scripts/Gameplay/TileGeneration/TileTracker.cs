using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.City;
using Gameplay.GameSpeed;
using UnityEngine;

public class TileTracker : MonoBehaviour
{
    private readonly Dictionary<float, Queue<Action>> _oneTimeProgressEvents = new();
    private readonly SortedSet<float> _pendingProgressThresholds = new();
    private readonly Dictionary<float, List<Action>> _progressEvents = new();
    private float _distanceTraveled;
    private int _previousTilePassed;
    private float _tileSize;

    public int TilesPassed { get; private set; }
    public float ProgressInTile { get; private set; }

    public event Action OnTileEndReached;

    /// <summary>
    ///     Добавляет событие, которое произойдет по достижении определенного прогресса.
    /// </summary>
    /// <param name="progressThreshold">Процент прохождения тайла, на котором будет вызвано событие.</param>
    /// <param name="callback">Делегат, который будет срабатывать.</param>
    /// <param name="oneTime">Определяет, будет ли событие удалено после выполненя, либо будет вызываться каждый тайл.</param>
    /// <param name="onCurrentTile">
    ///     Если = true, то сработает на текущем тайле.
    ///     Если игрок уже прошел этот момент, с этим параметром, все равно вызовется коллбек.
    /// </param>
    public void SubscribeToProgress(float progressThreshold, Action callback, bool oneTime, bool onCurrentTile)
    {
        if (IsCallbackContains(_oneTimeProgressEvents, progressThreshold, callback))
        {
            Debug.Log("This callback is already registered");
            return;
        }

        SubscribeToProgress(progressThreshold, callback, oneTime);

        if (onCurrentTile)
        {
            _pendingProgressThresholds.Add(progressThreshold);
        }
    }

    /// <summary>
    ///     Oтписка от события.
    /// </summary>
    /// <param name="progressThreshold">Прогресс на который были подписаны</param>
    /// <param name="callback">Делегат</param>
    public void UnsubscribeFromProgress(float progressThreshold, Action callback)
    {
        if (_progressEvents.TryGetValue(progressThreshold, out List<Action> callbacks))
        {
            callbacks.Remove(callback);
            if (callbacks.Count == 0)
            {
                _progressEvents.Remove(progressThreshold);
            }
        }
    }

    #region MonoBehaviour

    private void Awake()
    {
        _tileSize = CityTile.TileSize;
        _distanceTraveled = _tileSize / 2;
        _previousTilePassed = 0;
    }

    private void FixedUpdate()
    {
        UpdateDistance();
        CheckTileEnd();
        ProcessProgressEvents();
    }

    #endregion MonoBehaviour

    #region Implementation

    private void UpdateDistance()
    {
        _distanceTraveled += SpeedController.Speed * Time.fixedDeltaTime;
        TilesPassed = Mathf.FloorToInt(_distanceTraveled / _tileSize);
        ProgressInTile = _distanceTraveled % _tileSize / _tileSize;
    }

    private void CheckTileEnd()
    {
        if (TilesPassed > _previousTilePassed)
        {
            OnTileEndReached?.Invoke();
            _previousTilePassed = TilesPassed;
            ResetThresholds();
        }
    }

    /// <summary>
    ///     Проверяет прогресс и вызывает колбеки.
    ///     Удаляет одноразовые колбеки, после их выполнения
    /// </summary>
    private void ProcessProgressEvents()
    {
        while (_pendingProgressThresholds.Count > 0 && ProgressInTile >= _pendingProgressThresholds.Min)
        {
            float reachedThreshold = _pendingProgressThresholds.Min;
            _pendingProgressThresholds.Remove(reachedThreshold);

            InvokeLoopedCallbacks(_progressEvents, reachedThreshold);
            InvokeOneTimeCallbacks(_oneTimeProgressEvents, reachedThreshold);
        }
    }

    private void InvokeLoopedCallbacks(Dictionary<float, List<Action>> eventDict, float progress)
    {
        if (eventDict.TryGetValue(progress, out List<Action> callbacks))
        {
            foreach (Action callback in callbacks)
            {
                callback.Invoke();
            }
        }
    }

    private void InvokeOneTimeCallbacks(Dictionary<float, Queue<Action>> eventDict, float progress)
    {
        if (eventDict.TryGetValue(progress, out Queue<Action> callbacks))
        {
            while (callbacks.TryDequeue(out Action callback))
            {
                callback?.Invoke();
            }

            _oneTimeProgressEvents.Remove(progress);
        }
    }

    private void ResetThresholds()
    {
        _pendingProgressThresholds.Clear();

        foreach (float key in _progressEvents.Keys)
        {
            _pendingProgressThresholds.Add(key);
        }

        foreach (float key in _oneTimeProgressEvents.Keys)
        {
            _pendingProgressThresholds.Add(key);
        }
    }

    private bool IsCallbackContains<T>(Dictionary<float, T> eventDict, float progress, Action callback)
        where T : IEnumerable<Action> =>
        eventDict.TryGetValue(progress, out T callbacks) && callbacks.Contains(callback);

    private void SubscribeToProgress(float progressThreshold, Action callback, bool oneTime)
    {
        if (oneTime)
        {
            if (!_oneTimeProgressEvents.ContainsKey(progressThreshold))
            {
                _oneTimeProgressEvents[progressThreshold] = new Queue<Action>();
            }

            _oneTimeProgressEvents[progressThreshold].Enqueue(callback);
        }
        else
        {
            if (!_progressEvents.ContainsKey(progressThreshold))
            {
                _progressEvents[progressThreshold] = new List<Action>();
            }

            _progressEvents[progressThreshold].Add(callback);
        }
    }

    #endregion Implementation
}
