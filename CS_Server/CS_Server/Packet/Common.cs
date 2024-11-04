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
            "CgxDb21tb24ucHJvdG8SCFByb3RvY29sIlUKB1RQbGF5ZXISDgoGaXNTZWxm",
            "GAEgASgIEhAKCHBsYXllcklkGAIgASgFEgwKBHBvc1gYAyABKAISDAoEcG9z",
            "WRgEIAEoAhIMCgRwb3NaGAUgASgCQhmqAhZHb29nbGUuUHJvdG9idWYuQ29t",
            "bW9uYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Google.Protobuf.Common.TPlayer), global::Google.Protobuf.Common.TPlayer.Parser, new[]{ "IsSelf", "PlayerId", "PosX", "PosY", "PosZ" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class TPlayer : pb::IMessage<TPlayer>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<TPlayer> _parser = new pb::MessageParser<TPlayer>(() => new TPlayer());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<TPlayer> Parser { get { return _parser; } }

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
    public TPlayer() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TPlayer(TPlayer other) : this() {
      isSelf_ = other.isSelf_;
      playerId_ = other.playerId_;
      posX_ = other.posX_;
      posY_ = other.posY_;
      posZ_ = other.posZ_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public TPlayer Clone() {
      return new TPlayer(this);
    }

    /// <summary>Field number for the "isSelf" field.</summary>
    public const int IsSelfFieldNumber = 1;
    private bool isSelf_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool IsSelf {
      get { return isSelf_; }
      set {
        isSelf_ = value;
      }
    }

    /// <summary>Field number for the "playerId" field.</summary>
    public const int PlayerIdFieldNumber = 2;
    private int playerId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int PlayerId {
      get { return playerId_; }
      set {
        playerId_ = value;
      }
    }

    /// <summary>Field number for the "posX" field.</summary>
    public const int PosXFieldNumber = 3;
    private float posX_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float PosX {
      get { return posX_; }
      set {
        posX_ = value;
      }
    }

    /// <summary>Field number for the "posY" field.</summary>
    public const int PosYFieldNumber = 4;
    private float posY_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float PosY {
      get { return posY_; }
      set {
        posY_ = value;
      }
    }

    /// <summary>Field number for the "posZ" field.</summary>
    public const int PosZFieldNumber = 5;
    private float posZ_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public float PosZ {
      get { return posZ_; }
      set {
        posZ_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as TPlayer);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(TPlayer other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (IsSelf != other.IsSelf) return false;
      if (PlayerId != other.PlayerId) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(PosX, other.PosX)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(PosY, other.PosY)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(PosZ, other.PosZ)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (IsSelf != false) hash ^= IsSelf.GetHashCode();
      if (PlayerId != 0) hash ^= PlayerId.GetHashCode();
      if (PosX != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(PosX);
      if (PosY != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(PosY);
      if (PosZ != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(PosZ);
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
      if (IsSelf != false) {
        output.WriteRawTag(8);
        output.WriteBool(IsSelf);
      }
      if (PlayerId != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(PlayerId);
      }
      if (PosX != 0F) {
        output.WriteRawTag(29);
        output.WriteFloat(PosX);
      }
      if (PosY != 0F) {
        output.WriteRawTag(37);
        output.WriteFloat(PosY);
      }
      if (PosZ != 0F) {
        output.WriteRawTag(45);
        output.WriteFloat(PosZ);
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
      if (IsSelf != false) {
        output.WriteRawTag(8);
        output.WriteBool(IsSelf);
      }
      if (PlayerId != 0) {
        output.WriteRawTag(16);
        output.WriteInt32(PlayerId);
      }
      if (PosX != 0F) {
        output.WriteRawTag(29);
        output.WriteFloat(PosX);
      }
      if (PosY != 0F) {
        output.WriteRawTag(37);
        output.WriteFloat(PosY);
      }
      if (PosZ != 0F) {
        output.WriteRawTag(45);
        output.WriteFloat(PosZ);
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
      if (IsSelf != false) {
        size += 1 + 1;
      }
      if (PlayerId != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerId);
      }
      if (PosX != 0F) {
        size += 1 + 4;
      }
      if (PosY != 0F) {
        size += 1 + 4;
      }
      if (PosZ != 0F) {
        size += 1 + 4;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(TPlayer other) {
      if (other == null) {
        return;
      }
      if (other.IsSelf != false) {
        IsSelf = other.IsSelf;
      }
      if (other.PlayerId != 0) {
        PlayerId = other.PlayerId;
      }
      if (other.PosX != 0F) {
        PosX = other.PosX;
      }
      if (other.PosY != 0F) {
        PosY = other.PosY;
      }
      if (other.PosZ != 0F) {
        PosZ = other.PosZ;
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
            IsSelf = input.ReadBool();
            break;
          }
          case 16: {
            PlayerId = input.ReadInt32();
            break;
          }
          case 29: {
            PosX = input.ReadFloat();
            break;
          }
          case 37: {
            PosY = input.ReadFloat();
            break;
          }
          case 45: {
            PosZ = input.ReadFloat();
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
            IsSelf = input.ReadBool();
            break;
          }
          case 16: {
            PlayerId = input.ReadInt32();
            break;
          }
          case 29: {
            PosX = input.ReadFloat();
            break;
          }
          case 37: {
            PosY = input.ReadFloat();
            break;
          }
          case 45: {
            PosZ = input.ReadFloat();
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