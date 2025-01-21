using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.UserMessages;

namespace KnifeMute;

[MinimumApiVersion(294)]
public class KnifeMute : BasePlugin {
  public override string ModuleName => "KnifeMute";
  public override string ModuleVersion => "MSWS";

  private int lastKnifeTick;

  public override void Load(bool hotReload) {
    HookUserMessage(208, handleSoundEvent);

    VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnTakeDamage,
      HookMode.Pre);
  }


  public override void Unload(bool hotReload) {
    UnhookUserMessage(208, handleSoundEvent);

    VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(OnTakeDamage,
      HookMode.Pre);
  }

  private HookResult handleSoundEvent(UserMessage msg) {
    var soundEvent = msg.ReadUInt("soundevent_hash");
    if (soundEvent is not (SoundEvents.KNIFE_LEFTSTAB_BOTHSIDES
      or SoundEvents.KNIFE_RIGHTSTAB_BOTHSIDES))
      return HookResult.Continue;

    if (Server.TickCount - lastKnifeTick > 1) return HookResult.Continue;

    msg.SetUInt("soundevent_hash", SoundEvents.KNIFE_SWINGAIR_BOTHSIDES);
    return HookResult.Continue;
  }

  private HookResult OnTakeDamage(DynamicHook hook) {
    var entity = hook.GetParam<CEntityInstance>(0);
    if (!entity.IsValid || entity.DesignerName != "player")
      return HookResult.Continue;

    var damageInfo = hook.GetParam<CTakeDamageInfo>(1);

    var attackerIndex = damageInfo.Attacker.Value?.As<CBasePlayerPawn>()
     .Controller.Value;

    if (attackerIndex == null || !attackerIndex.IsValid)
      return HookResult.Continue;

    var pawn = entity.As<CCSPlayerPawn>();
    if (!pawn.IsValid) return HookResult.Continue;

    var attacker = Utilities.GetPlayerFromIndex((int)attackerIndex.Index);
    if (attacker == null || !attacker.IsValid) return HookResult.Continue;

    var victim = pawn.OriginalController.Get();
    if (victim == null || !victim.IsValid) return HookResult.Continue;

    if (attacker.Team != victim.Team) return HookResult.Continue;

    lastKnifeTick = Server.TickCount;
    return HookResult.Continue;
  }
}