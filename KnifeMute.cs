using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.UserMessages;

namespace KnifeMute;

[MinimumApiVersion(294)]
public class KnifeMute : BasePlugin {
  public override string ModuleName => "KnifeMute";
  public override string ModuleVersion => "MSWS";

  public override void Load(bool hotReload) {
    HookUserMessage(208, handleSoundEvent);
  }

  public override void Unload(bool hotReload) {
    UnhookUserMessage(208, handleSoundEvent);
  }

  private HookResult handleSoundEvent(UserMessage msg) {
    var soundEvent = msg.ReadUInt("soundevent_hash");
    Server.PrintToChatAll($"Sound Event: {soundEvent}");
    return HookResult.Continue;
  }
}