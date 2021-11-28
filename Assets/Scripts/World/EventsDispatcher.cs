using System;
using System.Collections.Generic;

public static class EventsDispatcher
{
    public static event Action onInventoryUpdated;
    public static event Action<List<DropsHolder>> onDropsChanged;
    public static event Action<Combat> onStatsChanged;

    public static void inventoryUpdated()
    {
        onInventoryUpdated?.Invoke();
    }

    public static void dropsChanged(List<DropsHolder> holders)
    {
        onDropsChanged?.Invoke(holders);
    }

    public static void statsChanged(Combat combat)
    {
        onStatsChanged?.Invoke(combat);
    }
}
