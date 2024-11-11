// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Common.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Google.Protobuf.Common {

  /// <summary>Holder for reflection information generated from Common.proto</summary>
  public static partial class CommonReflection {

    #region Descriptor
    /// <summary>File descriptor for Common.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CommonReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxDb21tb24ucHJvdG8SCFByb3RvY29sGgpFbnVtLnByb3RvInkKDFBvc2l0",
            "aW9uSW5mbxImCgVzdGF0ZRgBIAEoDjIXLlByb3RvY29sLkNyZWF0dXJlU3Rh",
            "dGUSIwoIbW92ZV9kaXIYAiABKA4yES5Qcm90b2NvbC5Nb3ZlRGlyEg0KBXBv",
            "c194GAMgASgFEg0KBXBvc195GAQgASgFIlYKClBsYXllckluZm8SEAoIcGxh",
            "eWVySWQYASABKAMSDAoEbmFtZRgCIAEoCRIoCghwb3NfaW5mbxgDIAEoCzIW",
            "LlByb3RvY29sLlBvc2l0aW9uSW5mbyIdCglTa2lsbEluZm8SEAoIc2tpbGxf",
            "aWQYASABKAVCGaoCFkdvb2dsZS5Qcm90b2J1Zi5Db21tb25iBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.Enum.EnumReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Common.PositionInfo), global::Google.Protobuf.Common.PositionInfo.Parser, new[]{ "State", "MoveDir", "PosX", "PosY" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Common.PlayerInfo), global::Google.Protobuf.Common.PlayerInfo.Parser, new[]{ "PlayerId", "Name", "PosInfo" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Common.SkillInfo), global::Google.Protobuf.Common.SkillInfo.Parser, new[]{ "SkillId" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class PositionInfo : pb::IMessage<PositionInfo>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<PositionInfo> _parser = new pb::MessageParser<PositionInfo>(() => new PositionInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<PositionInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Common.CommonReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PositionInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PositionInfo(PositionInfo other) : this() {
      state_ = other.state_;
      moveDir_ = other.moveDir_;
      posX_ = other.posX_;
      posY_ = other.posY_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PositionInfo Clone() {
      return new PositionInfo(this);
    }

    /// <summary>Field number for the "state" field.</summary>
    public const int StateFieldNumber = 1;
    private global::Google.Protobuf.Enum.CreatureState state_ = global::Google.Protobuf.Enum.CreatureState.Idle;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Google.Protobuf.Enum.CreatureState State {
      get { return state_; }
      set {
        state_ = value;
      }
    }

    /// <summary>Field number for the "move_dir" field.</summary>
    public const int MoveDirFieldNumber = 2;
    private global::Google.Protobuf.Enum.MoveDir moveDir_ = global::Google.Protobuf.Enum.MoveDir.Up;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Google.Protobuf.Enum.MoveDir MoveDir {
      get { return moveDir_; }
      set {
        moveDir_ = value;
      }
    }

    /// <summary>Field number for the "pos_x" field.</summary>
    public const int PosXFieldNumber = 3;
    private int posX_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int PosX {
      get { return posX_; }
      set {
        posX_ = value;
      }
    }

    /// <summary>Field number for the "pos_y" field.</summary>
    public const int PosYFieldNumber = 4;
    private int posY_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int PosY {
      get { return posY_; }
      set {
        posY_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as PositionInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(PositionInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (State != other.State) return false;
      if (MoveDir != other.MoveDir) return false;
      if (PosX != other.PosX) return false;
      if (PosY != other.PosY) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (State != global::Google.Protobuf.Enum.CreatureState.Idle) hash ^= State.GetHashCode();
      if (MoveDir != global::Google.Protobuf.Enum.MoveDir.Up) hash ^= MoveDir.GetHashCode();
      if (PosX != 0) hash ^= PosX.GetHashCode();
      if (PosY != 0) hash ^= PosY.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (State != global::Google.Protobuf.Enum.CreatureState.Idle) {
        output.WriteRawTag(8);
        output.WriteEnum((int) State);
      }
      if (MoveDir != global::Google.Protobuf.Enum.MoveDir.Up) {
        output.WriteRawTag(16);
        output.WriteEnum((int) MoveDir);
      }
      if (PosX != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(PosX);
      }
      if (PosY != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(PosY);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (State != global::Google.Protobuf.Enum.CreatureState.Idle) {
        output.WriteRawTag(8);
        output.WriteEnum((int) State);
      }
      if (MoveDir != global::Google.Protobuf.Enum.MoveDir.Up) {
        output.WriteRawTag(16);
        output.WriteEnum((int) MoveDir);
      }
      if (PosX != 0) {
        output.WriteRawTag(24);
        output.WriteInt32(PosX);
      }
      if (PosY != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(PosY);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (State != global::Google.Protobuf.Enum.CreatureState.Idle) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) State);
      }
      if (MoveDir != global::Google.Protobuf.Enum.MoveDir.Up) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) MoveDir);
      }
      if (PosX != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PosX);
      }
      if (PosY != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PosY);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(PositionInfo other) {
      if (other == null) {
        return;
      }
      if (other.State != global::Google.Protobuf.Enum.CreatureState.Idle) {
        State = other.State;
      }
      if (other.MoveDir != global::Google.Protobuf.Enum.MoveDir.Up) {
        MoveDir = other.MoveDir;
      }
      if (other.PosX != 0) {
        PosX = other.PosX;
      }
      if (other.PosY != 0) {
        PosY = other.PosY;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            State = (global::Google.Protobuf.Enum.CreatureState) input.ReadEnum();
            break;
          }
          case 16: {
            MoveDir = (global::Google.Protobuf.Enum.MoveDir) input.ReadEnum();
            break;
          }
          case 24: {
            PosX = input.ReadInt32();
            break;
          }
          case 32: {
            PosY = input.ReadInt32();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            State = (global::Google.Protobuf.Enum.CreatureState) input.ReadEnum();
            break;
          }
          case 16: {
            MoveDir = (global::Google.Protobuf.Enum.MoveDir) input.ReadEnum();
            break;
          }
          case 24: {
            PosX = input.ReadInt32();
            break;
          }
          case 32: {
            PosY = input.ReadInt32();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class PlayerInfo : pb::IMessage<PlayerInfo>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<PlayerInfo> _parser = new pb::MessageParser<PlayerInfo>(() => new PlayerInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<PlayerInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Common.CommonReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PlayerInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PlayerInfo(PlayerInfo other) : this() {
      playerId_ = other.playerId_;
      name_ = other.name_;
      posInfo_ = other.posInfo_ != null ? other.posInfo_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public PlayerInfo Clone() {
      return new PlayerInfo(this);
    }

    /// <summary>Field number for the "playerId" field.</summary>
    public const int PlayerIdFieldNumber = 1;
    private long playerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public long PlayerId {
      get { return playerId_; }
      set {
        playerId_ = value;
      }
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 2;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "pos_info" field.</summary>
    public const int PosInfoFieldNumber = 3;
    private global::Google.Protobuf.Common.PositionInfo posInfo_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Google.Protobuf.Common.PositionInfo PosInfo {
      get { return posInfo_; }
      set {
        posInfo_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as PlayerInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(PlayerInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (PlayerId != other.PlayerId) return false;
      if (Name != other.Name) return false;
      if (!object.Equals(PosInfo, other.PosInfo)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (PlayerId != 0L) hash ^= PlayerId.GetHashCode();
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (posInfo_ != null) hash ^= PosInfo.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (PlayerId != 0L) {
        output.WriteRawTag(8);
        output.WriteInt64(PlayerId);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Name);
      }
      if (posInfo_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(PosInfo);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (PlayerId != 0L) {
        output.WriteRawTag(8);
        output.WriteInt64(PlayerId);
      }
      if (Name.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Name);
      }
      if (posInfo_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(PosInfo);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (PlayerId != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(PlayerId);
      }
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (posInfo_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(PosInfo);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(PlayerInfo other) {
      if (other == null) {
        return;
      }
      if (other.PlayerId != 0L) {
        PlayerId = other.PlayerId;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.posInfo_ != null) {
        if (posInfo_ == null) {
          PosInfo = new global::Google.Protobuf.Common.PositionInfo();
        }
        PosInfo.MergeFrom(other.PosInfo);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            PlayerId = input.ReadInt64();
            break;
          }
          case 18: {
            Name = input.ReadString();
            break;
          }
          case 26: {
            if (posInfo_ == null) {
              PosInfo = new global::Google.Protobuf.Common.PositionInfo();
            }
            input.ReadMessage(PosInfo);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            PlayerId = input.ReadInt64();
            break;
          }
          case 18: {
            Name = input.ReadString();
            break;
          }
          case 26: {
            if (posInfo_ == null) {
              PosInfo = new global::Google.Protobuf.Common.PositionInfo();
            }
            input.ReadMessage(PosInfo);
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class SkillInfo : pb::IMessage<SkillInfo>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<SkillInfo> _parser = new pb::MessageParser<SkillInfo>(() => new SkillInfo());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<SkillInfo> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Google.Protobuf.Common.CommonReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SkillInfo() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SkillInfo(SkillInfo other) : this() {
      skillId_ = other.skillId_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public SkillInfo Clone() {
      return new SkillInfo(this);
    }

    /// <summary>Field number for the "skill_id" field.</summary>
    public const int SkillIdFieldNumber = 1;
    private int skillId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int SkillId {
      get { return skillId_; }
      set {
        skillId_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as SkillInfo);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(SkillInfo other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (SkillId != other.SkillId) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (SkillId != 0) hash ^= SkillId.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (SkillId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(SkillId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (SkillId != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(SkillId);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (SkillId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(SkillId);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(SkillInfo other) {
      if (other == null) {
        return;
      }
      if (other.SkillId != 0) {
        SkillId = other.SkillId;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            SkillId = input.ReadInt32();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            SkillId = input.ReadInt32();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
