// Code generated by protoc-gen-go. DO NOT EDIT.
// versions:
// 	protoc-gen-go v1.28.0
// 	protoc        v3.21.6
// source: asyncrpc.proto

package asyncnetworkengine

import (
	protoreflect "google.golang.org/protobuf/reflect/protoreflect"
	protoimpl "google.golang.org/protobuf/runtime/protoimpl"
	reflect "reflect"
	sync "sync"
)

const (
	// Verify that this generated code is sufficiently up-to-date.
	_ = protoimpl.EnforceVersion(20 - protoimpl.MinVersion)
	// Verify that runtime/protoimpl is sufficiently up-to-date.
	_ = protoimpl.EnforceVersion(protoimpl.MaxVersion - 20)
)

type Transmission int32

const (
	Transmission_NotSpecified                       Transmission = 0
	Transmission_ErrorConnection                    Transmission = 1
	Transmission_ErrorProtocol                      Transmission = 2
	Transmission_ErrorParser                        Transmission = 3
	Transmission_Success                            Transmission = 200 // StatusOK
	Transmission_ErrorNoData                        Transmission = 204 //StatusNoContent
	Transmission_ErrorDecode                        Transmission = 400 //StatusBadRequest
	Transmission_ErrorUnauthorized                  Transmission = 401 //Unauthorized
	Transmission_ErrorForbidden                     Transmission = 403 //Forbidden
	Transmission_ErrorMethodNotAllowed              Transmission = 405 //StatusMethodNotAllowed
	Transmission_ErrorRequestTimeout                Transmission = 408 //StatusRequestTimeout
	Transmission_ErrorEncode                        Transmission = 421 //Misdirected Request
	Transmission_ErrorInternalServer                Transmission = 500 //StatusInternalServerError
	Transmission_ErrorNotImplemented                Transmission = 501 //StatusNotImplemented
	Transmission_ErrorNetworkAuthenticationRequired Transmission = 511 //NetworkAuthenticationRequired
)

// Enum value maps for Transmission.
var (
	Transmission_name = map[int32]string{
		0:   "NotSpecified",
		1:   "ErrorConnection",
		2:   "ErrorProtocol",
		3:   "ErrorParser",
		200: "Success",
		204: "ErrorNoData",
		400: "ErrorDecode",
		401: "ErrorUnauthorized",
		403: "ErrorForbidden",
		405: "ErrorMethodNotAllowed",
		408: "ErrorRequestTimeout",
		421: "ErrorEncode",
		500: "ErrorInternalServer",
		501: "ErrorNotImplemented",
		511: "ErrorNetworkAuthenticationRequired",
	}
	Transmission_value = map[string]int32{
		"NotSpecified":                       0,
		"ErrorConnection":                    1,
		"ErrorProtocol":                      2,
		"ErrorParser":                        3,
		"Success":                            200,
		"ErrorNoData":                        204,
		"ErrorDecode":                        400,
		"ErrorUnauthorized":                  401,
		"ErrorForbidden":                     403,
		"ErrorMethodNotAllowed":              405,
		"ErrorRequestTimeout":                408,
		"ErrorEncode":                        421,
		"ErrorInternalServer":                500,
		"ErrorNotImplemented":                501,
		"ErrorNetworkAuthenticationRequired": 511,
	}
)

func (x Transmission) Enum() *Transmission {
	p := new(Transmission)
	*p = x
	return p
}

func (x Transmission) String() string {
	return protoimpl.X.EnumStringOf(x.Descriptor(), protoreflect.EnumNumber(x))
}

func (Transmission) Descriptor() protoreflect.EnumDescriptor {
	return file_asyncrpc_proto_enumTypes[0].Descriptor()
}

func (Transmission) Type() protoreflect.EnumType {
	return &file_asyncrpc_proto_enumTypes[0]
}

func (x Transmission) Number() protoreflect.EnumNumber {
	return protoreflect.EnumNumber(x)
}

// Deprecated: Use Transmission.Descriptor instead.
func (Transmission) EnumDescriptor() ([]byte, []int) {
	return file_asyncrpc_proto_rawDescGZIP(), []int{0}
}

type GenericRequest struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Messege string `protobuf:"bytes,1,opt,name=Messege,proto3" json:"Messege,omitempty"`
}

func (x *GenericRequest) Reset() {
	*x = GenericRequest{}
	if protoimpl.UnsafeEnabled {
		mi := &file_asyncrpc_proto_msgTypes[0]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *GenericRequest) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*GenericRequest) ProtoMessage() {}

func (x *GenericRequest) ProtoReflect() protoreflect.Message {
	mi := &file_asyncrpc_proto_msgTypes[0]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use GenericRequest.ProtoReflect.Descriptor instead.
func (*GenericRequest) Descriptor() ([]byte, []int) {
	return file_asyncrpc_proto_rawDescGZIP(), []int{0}
}

func (x *GenericRequest) GetMessege() string {
	if x != nil {
		return x.Messege
	}
	return ""
}

type GenericResponse struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Messege string `protobuf:"bytes,2,opt,name=Messege,proto3" json:"Messege,omitempty"`
}

func (x *GenericResponse) Reset() {
	*x = GenericResponse{}
	if protoimpl.UnsafeEnabled {
		mi := &file_asyncrpc_proto_msgTypes[1]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *GenericResponse) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*GenericResponse) ProtoMessage() {}

func (x *GenericResponse) ProtoReflect() protoreflect.Message {
	mi := &file_asyncrpc_proto_msgTypes[1]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use GenericResponse.ProtoReflect.Descriptor instead.
func (*GenericResponse) Descriptor() ([]byte, []int) {
	return file_asyncrpc_proto_rawDescGZIP(), []int{1}
}

func (x *GenericResponse) GetMessege() string {
	if x != nil {
		return x.Messege
	}
	return ""
}

type GenericErrorResponse struct {
	state         protoimpl.MessageState
	sizeCache     protoimpl.SizeCache
	unknownFields protoimpl.UnknownFields

	Error string `protobuf:"bytes,1,opt,name=Error,proto3" json:"Error,omitempty"`
}

func (x *GenericErrorResponse) Reset() {
	*x = GenericErrorResponse{}
	if protoimpl.UnsafeEnabled {
		mi := &file_asyncrpc_proto_msgTypes[2]
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		ms.StoreMessageInfo(mi)
	}
}

func (x *GenericErrorResponse) String() string {
	return protoimpl.X.MessageStringOf(x)
}

func (*GenericErrorResponse) ProtoMessage() {}

func (x *GenericErrorResponse) ProtoReflect() protoreflect.Message {
	mi := &file_asyncrpc_proto_msgTypes[2]
	if protoimpl.UnsafeEnabled && x != nil {
		ms := protoimpl.X.MessageStateOf(protoimpl.Pointer(x))
		if ms.LoadMessageInfo() == nil {
			ms.StoreMessageInfo(mi)
		}
		return ms
	}
	return mi.MessageOf(x)
}

// Deprecated: Use GenericErrorResponse.ProtoReflect.Descriptor instead.
func (*GenericErrorResponse) Descriptor() ([]byte, []int) {
	return file_asyncrpc_proto_rawDescGZIP(), []int{2}
}

func (x *GenericErrorResponse) GetError() string {
	if x != nil {
		return x.Error
	}
	return ""
}

var File_asyncrpc_proto protoreflect.FileDescriptor

var file_asyncrpc_proto_rawDesc = []byte{
	0x0a, 0x0e, 0x61, 0x73, 0x79, 0x6e, 0x63, 0x72, 0x70, 0x63, 0x2e, 0x70, 0x72, 0x6f, 0x74, 0x6f,
	0x12, 0x20, 0x47, 0x61, 0x6d, 0x65, 0x57, 0x6f, 0x72, 0x6b, 0x73, 0x74, 0x6f, 0x72, 0x65, 0x2e,
	0x41, 0x73, 0x79, 0x6e, 0x63, 0x4e, 0x65, 0x74, 0x77, 0x6f, 0x72, 0x6b, 0x45, 0x6e, 0x67, 0x69,
	0x6e, 0x65, 0x22, 0x2a, 0x0a, 0x0e, 0x47, 0x65, 0x6e, 0x65, 0x72, 0x69, 0x63, 0x52, 0x65, 0x71,
	0x75, 0x65, 0x73, 0x74, 0x12, 0x18, 0x0a, 0x07, 0x4d, 0x65, 0x73, 0x73, 0x65, 0x67, 0x65, 0x18,
	0x01, 0x20, 0x01, 0x28, 0x09, 0x52, 0x07, 0x4d, 0x65, 0x73, 0x73, 0x65, 0x67, 0x65, 0x22, 0x2b,
	0x0a, 0x0f, 0x47, 0x65, 0x6e, 0x65, 0x72, 0x69, 0x63, 0x52, 0x65, 0x73, 0x70, 0x6f, 0x6e, 0x73,
	0x65, 0x12, 0x18, 0x0a, 0x07, 0x4d, 0x65, 0x73, 0x73, 0x65, 0x67, 0x65, 0x18, 0x02, 0x20, 0x01,
	0x28, 0x09, 0x52, 0x07, 0x4d, 0x65, 0x73, 0x73, 0x65, 0x67, 0x65, 0x22, 0x2c, 0x0a, 0x14, 0x47,
	0x65, 0x6e, 0x65, 0x72, 0x69, 0x63, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x52, 0x65, 0x73, 0x70, 0x6f,
	0x6e, 0x73, 0x65, 0x12, 0x14, 0x0a, 0x05, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x18, 0x01, 0x20, 0x01,
	0x28, 0x09, 0x52, 0x05, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x2a, 0xdd, 0x02, 0x0a, 0x0c, 0x54, 0x72,
	0x61, 0x6e, 0x73, 0x6d, 0x69, 0x73, 0x73, 0x69, 0x6f, 0x6e, 0x12, 0x10, 0x0a, 0x0c, 0x4e, 0x6f,
	0x74, 0x53, 0x70, 0x65, 0x63, 0x69, 0x66, 0x69, 0x65, 0x64, 0x10, 0x00, 0x12, 0x13, 0x0a, 0x0f,
	0x45, 0x72, 0x72, 0x6f, 0x72, 0x43, 0x6f, 0x6e, 0x6e, 0x65, 0x63, 0x74, 0x69, 0x6f, 0x6e, 0x10,
	0x01, 0x12, 0x11, 0x0a, 0x0d, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x50, 0x72, 0x6f, 0x74, 0x6f, 0x63,
	0x6f, 0x6c, 0x10, 0x02, 0x12, 0x0f, 0x0a, 0x0b, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x50, 0x61, 0x72,
	0x73, 0x65, 0x72, 0x10, 0x03, 0x12, 0x0c, 0x0a, 0x07, 0x53, 0x75, 0x63, 0x63, 0x65, 0x73, 0x73,
	0x10, 0xc8, 0x01, 0x12, 0x10, 0x0a, 0x0b, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x4e, 0x6f, 0x44, 0x61,
	0x74, 0x61, 0x10, 0xcc, 0x01, 0x12, 0x10, 0x0a, 0x0b, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x44, 0x65,
	0x63, 0x6f, 0x64, 0x65, 0x10, 0x90, 0x03, 0x12, 0x16, 0x0a, 0x11, 0x45, 0x72, 0x72, 0x6f, 0x72,
	0x55, 0x6e, 0x61, 0x75, 0x74, 0x68, 0x6f, 0x72, 0x69, 0x7a, 0x65, 0x64, 0x10, 0x91, 0x03, 0x12,
	0x13, 0x0a, 0x0e, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x46, 0x6f, 0x72, 0x62, 0x69, 0x64, 0x64, 0x65,
	0x6e, 0x10, 0x93, 0x03, 0x12, 0x1a, 0x0a, 0x15, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x4d, 0x65, 0x74,
	0x68, 0x6f, 0x64, 0x4e, 0x6f, 0x74, 0x41, 0x6c, 0x6c, 0x6f, 0x77, 0x65, 0x64, 0x10, 0x95, 0x03,
	0x12, 0x18, 0x0a, 0x13, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x52, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74,
	0x54, 0x69, 0x6d, 0x65, 0x6f, 0x75, 0x74, 0x10, 0x98, 0x03, 0x12, 0x10, 0x0a, 0x0b, 0x45, 0x72,
	0x72, 0x6f, 0x72, 0x45, 0x6e, 0x63, 0x6f, 0x64, 0x65, 0x10, 0xa5, 0x03, 0x12, 0x18, 0x0a, 0x13,
	0x45, 0x72, 0x72, 0x6f, 0x72, 0x49, 0x6e, 0x74, 0x65, 0x72, 0x6e, 0x61, 0x6c, 0x53, 0x65, 0x72,
	0x76, 0x65, 0x72, 0x10, 0xf4, 0x03, 0x12, 0x18, 0x0a, 0x13, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x4e,
	0x6f, 0x74, 0x49, 0x6d, 0x70, 0x6c, 0x65, 0x6d, 0x65, 0x6e, 0x74, 0x65, 0x64, 0x10, 0xf5, 0x03,
	0x12, 0x27, 0x0a, 0x22, 0x45, 0x72, 0x72, 0x6f, 0x72, 0x4e, 0x65, 0x74, 0x77, 0x6f, 0x72, 0x6b,
	0x41, 0x75, 0x74, 0x68, 0x65, 0x6e, 0x74, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6f, 0x6e, 0x52, 0x65,
	0x71, 0x75, 0x69, 0x72, 0x65, 0x64, 0x10, 0xff, 0x03, 0x42, 0x47, 0x48, 0x03, 0x5a, 0x43, 0x67,
	0x69, 0x74, 0x68, 0x75, 0x62, 0x2e, 0x63, 0x6f, 0x6d, 0x2f, 0x47, 0x61, 0x6d, 0x65, 0x57, 0x6f,
	0x72, 0x6b, 0x73, 0x74, 0x6f, 0x72, 0x65, 0x2f, 0x61, 0x73, 0x79, 0x6e, 0x63, 0x2d, 0x6e, 0x65,
	0x74, 0x77, 0x6f, 0x72, 0x6b, 0x2d, 0x65, 0x6e, 0x67, 0x69, 0x6e, 0x65, 0x2d, 0x67, 0x6f, 0x3b,
	0x61, 0x73, 0x79, 0x6e, 0x63, 0x6e, 0x65, 0x74, 0x77, 0x6f, 0x72, 0x6b, 0x65, 0x6e, 0x67, 0x69,
	0x6e, 0x65, 0x62, 0x06, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x33,
}

var (
	file_asyncrpc_proto_rawDescOnce sync.Once
	file_asyncrpc_proto_rawDescData = file_asyncrpc_proto_rawDesc
)

func file_asyncrpc_proto_rawDescGZIP() []byte {
	file_asyncrpc_proto_rawDescOnce.Do(func() {
		file_asyncrpc_proto_rawDescData = protoimpl.X.CompressGZIP(file_asyncrpc_proto_rawDescData)
	})
	return file_asyncrpc_proto_rawDescData
}

var file_asyncrpc_proto_enumTypes = make([]protoimpl.EnumInfo, 1)
var file_asyncrpc_proto_msgTypes = make([]protoimpl.MessageInfo, 3)
var file_asyncrpc_proto_goTypes = []interface{}{
	(Transmission)(0),            // 0: GameWorkstore.AsyncNetworkEngine.Transmission
	(*GenericRequest)(nil),       // 1: GameWorkstore.AsyncNetworkEngine.GenericRequest
	(*GenericResponse)(nil),      // 2: GameWorkstore.AsyncNetworkEngine.GenericResponse
	(*GenericErrorResponse)(nil), // 3: GameWorkstore.AsyncNetworkEngine.GenericErrorResponse
}
var file_asyncrpc_proto_depIdxs = []int32{
	0, // [0:0] is the sub-list for method output_type
	0, // [0:0] is the sub-list for method input_type
	0, // [0:0] is the sub-list for extension type_name
	0, // [0:0] is the sub-list for extension extendee
	0, // [0:0] is the sub-list for field type_name
}

func init() { file_asyncrpc_proto_init() }
func file_asyncrpc_proto_init() {
	if File_asyncrpc_proto != nil {
		return
	}
	if !protoimpl.UnsafeEnabled {
		file_asyncrpc_proto_msgTypes[0].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*GenericRequest); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_asyncrpc_proto_msgTypes[1].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*GenericResponse); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
		file_asyncrpc_proto_msgTypes[2].Exporter = func(v interface{}, i int) interface{} {
			switch v := v.(*GenericErrorResponse); i {
			case 0:
				return &v.state
			case 1:
				return &v.sizeCache
			case 2:
				return &v.unknownFields
			default:
				return nil
			}
		}
	}
	type x struct{}
	out := protoimpl.TypeBuilder{
		File: protoimpl.DescBuilder{
			GoPackagePath: reflect.TypeOf(x{}).PkgPath(),
			RawDescriptor: file_asyncrpc_proto_rawDesc,
			NumEnums:      1,
			NumMessages:   3,
			NumExtensions: 0,
			NumServices:   0,
		},
		GoTypes:           file_asyncrpc_proto_goTypes,
		DependencyIndexes: file_asyncrpc_proto_depIdxs,
		EnumInfos:         file_asyncrpc_proto_enumTypes,
		MessageInfos:      file_asyncrpc_proto_msgTypes,
	}.Build()
	File_asyncrpc_proto = out.File
	file_asyncrpc_proto_rawDesc = nil
	file_asyncrpc_proto_goTypes = nil
	file_asyncrpc_proto_depIdxs = nil
}
