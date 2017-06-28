using ASBinCode;
using ASBinCode.rtti;
using ASRuntime;
using ASRuntime.nativefuncs;
using System;
using System.Collections;
using System.Collections.Generic;
namespace ASCTest.regNativeFunctions
{
	class system_security_permissions_FileDialogPermissionAccess_buildin
	{
		public static void regNativeFunctions(CSWC bin)
		{
			bin.regNativeFunction(LinkSystem_Buildin.getCreator("system_security_permissions_FileDialogPermissionAccess_creator", default(System.Security.Permissions.FileDialogPermissionAccess)));
			bin.regNativeFunction(new system_security_permissions_FileDialogPermissionAccess_ctor());
			bin.regNativeFunction(LinkSystem_Buildin.getStruct_static_field_getter("system_security_permissions_FileDialogPermissionAccess_None_getter",()=>{ return System.Security.Permissions.FileDialogPermissionAccess.None;}));
			bin.regNativeFunction(LinkSystem_Buildin.getStruct_static_field_getter("system_security_permissions_FileDialogPermissionAccess_Open_getter",()=>{ return System.Security.Permissions.FileDialogPermissionAccess.Open;}));
			bin.regNativeFunction(LinkSystem_Buildin.getStruct_static_field_getter("system_security_permissions_FileDialogPermissionAccess_Save_getter",()=>{ return System.Security.Permissions.FileDialogPermissionAccess.Save;}));
			bin.regNativeFunction(LinkSystem_Buildin.getStruct_static_field_getter("system_security_permissions_FileDialogPermissionAccess_OpenSave_getter",()=>{ return System.Security.Permissions.FileDialogPermissionAccess.OpenSave;}));
			bin.regNativeFunction(new system_security_permissions_FileDialogPermissionAccess_operator_bitOr());
		}

		class system_security_permissions_FileDialogPermissionAccess_ctor : NativeFunctionBase
		{
			public system_security_permissions_FileDialogPermissionAccess_ctor()
			{
				para = new List<RunTimeDataType>();
			}

			public override bool isMethod
			{
				get
				{
					return true;
				}
			}

			public override string name
			{
				get
				{
					return "system_security_permissions_FileDialogPermissionAccess_ctor";
				}
			}

			List<RunTimeDataType> para;
			public override List<RunTimeDataType> parameters
			{
				get
				{
					return para;
				}
			}

			public override RunTimeDataType returnType
			{
				get
				{
					return RunTimeDataType.rt_void;
				}
			}

			public override RunTimeValueBase execute(RunTimeValueBase thisObj, SLOT[] argements, object stackframe, out string errormessage, out int errorno)
			{
				errormessage = null; errorno = 0;
				return ASBinCode.rtData.rtUndefined.undefined;

			}
		}

		class system_security_permissions_FileDialogPermissionAccess_operator_bitOr : ASRuntime.nativefuncs.NativeConstParameterFunction
		{
			public system_security_permissions_FileDialogPermissionAccess_operator_bitOr() : base(2)
			{
				para = new List<RunTimeDataType>();
				para.Add(RunTimeDataType.rt_void);
				para.Add(RunTimeDataType.rt_void);
			}

			public override bool isMethod
			{
				get
				{
					return true;
				}
			}

			public override string name
			{
				get
				{
					return "system_security_permissions_FileDialogPermissionAccess_operator_bitOr";
				}
			}

			List<RunTimeDataType> para;
			public override List<RunTimeDataType> parameters
			{
				get
				{
					return para;
				}
			}

			public override RunTimeDataType returnType
			{
				get
				{
					return RunTimeDataType.rt_void;
				}
			}

			public override void execute3(RunTimeValueBase thisObj, FunctionDefine functionDefine, SLOT returnSlot, SourceToken token, StackFrame stackframe, out bool success)
			{
				System.Security.Permissions.FileDialogPermissionAccess ts1;

				if (argements[0].rtType == RunTimeDataType.rt_null)
				{
					ts1 = default(System.Security.Permissions.FileDialogPermissionAccess);
				}
				else
				{
					LinkObj<object> argObj = (LinkObj<object>)((ASBinCode.rtData.rtObject)argements[0]).value;
					ts1 = (System.Security.Permissions.FileDialogPermissionAccess)argObj.value;
				}

				System.Security.Permissions.FileDialogPermissionAccess ts2;

				if (argements[1].rtType == RunTimeDataType.rt_null)
				{
					ts2 = default(System.Security.Permissions.FileDialogPermissionAccess);
				}
				else
				{
					LinkObj<object> argObj = (LinkObj<object>)((ASBinCode.rtData.rtObject)argements[1]).value;
					ts2 = (System.Security.Permissions.FileDialogPermissionAccess)argObj.value;
				}

				((StackSlot)returnSlot).setLinkObjectValue(
					bin.getClassByRunTimeDataType(functionDefine.signature.returnType), stackframe.player, ts1 | ts2);

				success = true;
			}
		}

	}
}