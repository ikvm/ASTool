﻿
using ASBinCode;
using ASBinCode.rtData;
using ASBinCode.rtti;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASRuntime.operators
{
    class OpForIn
    {
        public static void forin_get_enumerator(Player player, StackFrame frame, OpStep step, IRunTimeScope scope)
        {

            StackSlot save = (StackSlot)step.reg.getISlot(scope);

            var obj = step.arg1.getValue(scope);
            if (obj.rtType > RunTimeDataType.unknown)
            {
                rtObject rtObj = (rtObject)obj;

                if(ClassMemberFinder.isInherits(rtObj.value._class, 
                    player.swc.primitive_to_class_table[RunTimeDataType.rt_array] ))
                {
                    rtArray arr = (rtArray)rtObj.value.memberData[0].getValue();
                    save.cache_enumerator = getArrayForIn(arr);
                }
                else
                {
                    IEnumerator<IRunTimeValue> forinenum = getForinIEnumerator(player, rtObj.value, frame, step, scope);
                    save.cache_enumerator = forinenum;   
                }
            }

            frame.endStep();
        }


        public static void foreach_get_enumerator(Player player, StackFrame frame, OpStep step, IRunTimeScope scope)
        {
            StackSlot save = (StackSlot)step.reg.getISlot(scope);

            var obj = step.arg1.getValue(scope);
            if (obj.rtType > RunTimeDataType.unknown)
            {
                rtObject rtObj = (rtObject)obj;

                if (ClassMemberFinder.isInherits(rtObj.value._class,
                    player.swc.primitive_to_class_table[RunTimeDataType.rt_array]))
                {
                    rtArray arr = (rtArray)rtObj.value.memberData[0].getValue();
                    save.cache_enumerator = getArrayForEach(arr);
                }
                else
                {
                    IEnumerator<IRunTimeValue> forinenum = getForEach_IEnumerator(player, rtObj.value, frame, step, scope);
                    save.cache_enumerator = forinenum;
                }
            }

            frame.endStep();
        }


        public static void enumerator_movenext(Player player, StackFrame frame, OpStep step, IRunTimeScope scope)
        {
            StackSlot slot = (StackSlot)((Register)step.arg1).getISlot(scope);

            if (slot.cache_enumerator.MoveNext())
            {
                step.reg.getISlot(scope).setValue(rtBoolean.True);
            }
            else
            {
                step.reg.getISlot(scope).setValue(rtBoolean.False);
            }

            frame.endStep(step);
        }
        public static void enumerator_current(Player player, StackFrame frame, OpStep step, IRunTimeScope scope)
        {
            StackSlot slot = (StackSlot)((Register)step.arg1).getISlot(scope);

            step.reg.getISlot(scope).directSet(slot.cache_enumerator.Current);

            frame.endStep(step);
        }

        public static void enumerator_close(Player player, StackFrame frame, OpStep step, IRunTimeScope scope)
        {
            StackSlot slot = (StackSlot)((Register)step.arg1).getISlot(scope);

            slot.cache_enumerator.Dispose();
            slot.cache_enumerator = null;

            frame.endStep(step);
        }

        private static IEnumerator<IRunTimeValue> getArrayForIn(rtArray arr)
        {
            int length = arr.innerArray.Count;
            for (int i = 0; i < length; i++)
            {
                yield return new rtInt(i);
            }
        }

        private static IEnumerator<IRunTimeValue> getArrayForEach(rtArray arr)
        {
            int length = arr.innerArray.Count;
            for (int i = 0; i < length; i++)
            {
                yield return arr.innerArray[i];
            }
        }

        private static IEnumerator<IRunTimeValue> getForinIEnumerator(
            Player player,ASBinCode.rtti.Object obj ,StackFrame frame, OpStep step, IRunTimeScope scope)
        {
            if (obj is ASBinCode.rtti.DynamicObject)
            {
                ASBinCode.rtti.DynamicObject dobj = (ASBinCode.rtti.DynamicObject)obj;
                {
                    var k = dobj.eachSlot();
                    while (k.MoveNext())
                    {
                        var c = k.Current;
                        DynamicPropertySlot ds = c as DynamicPropertySlot;
                        if (c != null)
                        {
                            yield return new rtString(ds._propname);
                        }
                    }
                }

                if (obj is DictionaryObject)
                {
                    DictionaryObject dictObj = (DictionaryObject)obj;
                    var k = dictObj.eachDictSlot();
                    while (k.MoveNext())
                    {
                        var c = k.Current;
                        DictionarySlot ds = c as DictionarySlot;
                        if (c != null)
                        {
                            yield return ds._key.key;
                        }
                    }

                }

                //***再到原型链中查找
                if (dobj._prototype_ != null)
                {
                    var protoObj = dobj._prototype_;
                    //****_prototype_的类型，只可能是Function对象或Class对象 Class对象尚未实现
                    if (protoObj._class.classid == 10) //Function 
                    {
                        dobj = (DynamicObject)((rtObject)protoObj.memberData[1].getValue()).value;
                        var res = getForinIEnumerator(player, dobj, frame, step, scope);
                        while (res.MoveNext())
                        {
                            yield return res.Current;
                        }
                    }
                    else if (protoObj._class.classid == 1) //搜索到根Object
                    {
                        //***根Object有继承自Class的prototype,再没有就没有了
                        dobj = (DynamicObject)((rtObject)protoObj.memberData[0].getValue()).value;
                        {
                            var k = dobj.eachSlot();
                            while (k.MoveNext())
                            {
                                var c = k.Current;
                                DynamicPropertySlot ds = c as DynamicPropertySlot;
                                if (c != null)
                                {
                                    yield return new rtString(ds._propname);
                                }
                            }
                        }
                        yield break;
                    }
                    else if (protoObj._class.staticClass == null)
                    {
                        dobj = (DynamicObject)((rtObject)protoObj.memberData[0].getValue()).value;
                        var res = getForinIEnumerator(player, dobj, frame, step, scope);
                        while (res.MoveNext())
                        {
                            yield return res.Current;
                        }
                    }
                    else
                    {

                        frame.throwError((new error.InternalError(step.token,
                             "遭遇了异常的_prototype_"
                             )));
                        yield break;
                    }
                }
            }
            else if (obj is ASBinCode.rtti.Object)
            {
                var dobj = ((ASBinCode.rtti.DynamicObject)
                    frame.player.static_instance[obj._class.staticClass.classid].value);

                dobj = (ASBinCode.rtti.DynamicObject)((rtObject)dobj.memberData[0].getValue()).value;
                var res = getForinIEnumerator(player, dobj, frame, step, scope);
                while (res.MoveNext())
                {
                    yield return res.Current;
                }
            }

            yield break;
        }


        private static IEnumerator<IRunTimeValue> getForEach_IEnumerator(
            Player player, ASBinCode.rtti.Object obj, StackFrame frame, OpStep step, IRunTimeScope scope)
        {
            if (obj is ASBinCode.rtti.DynamicObject)
            {
                ASBinCode.rtti.DynamicObject dobj = (ASBinCode.rtti.DynamicObject)obj;
                {
                    var k = dobj.eachSlot();
                    while (k.MoveNext())
                    {
                        var c = k.Current;
                        DynamicPropertySlot ds = c as DynamicPropertySlot;
                        if (c != null)
                        {
                            yield return ds.getValue(); //new rtString(ds._propname);
                        }
                    }
                }

                if (obj is DictionaryObject)
                {
                    DictionaryObject dictObj = (DictionaryObject)obj;
                    var k = dictObj.eachDictSlot();
                    while (k.MoveNext())
                    {
                        var c = k.Current;
                        DictionarySlot ds = c as DictionarySlot;
                        if (c != null)
                        {
                            yield return ds.getValue(); //ds._key.key;
                        }
                    }

                }

                //***再到原型链中查找
                if (dobj._prototype_ != null)
                {
                    var protoObj = dobj._prototype_;
                    //****_prototype_的类型，只可能是Function对象或Class对象 Class对象尚未实现
                    if (protoObj._class.classid == 10) //Function 
                    {
                        dobj = (DynamicObject)((rtObject)protoObj.memberData[1].getValue()).value;
                        var res = getForEach_IEnumerator(player, dobj, frame, step, scope);
                        while (res.MoveNext())
                        {
                            yield return res.Current;
                        }
                    }
                    else if (protoObj._class.classid == 1) //搜索到根Object
                    {
                        //***根Object有继承自Class的prototype,再没有就没有了
                        dobj = (DynamicObject)((rtObject)protoObj.memberData[0].getValue()).value;
                        {
                            var k = dobj.eachSlot();
                            while (k.MoveNext())
                            {
                                var c = k.Current;
                                DynamicPropertySlot ds = c as DynamicPropertySlot;
                                if (c != null)
                                {
                                    yield return ds.getValue(); //new rtString(ds._propname);
                                }
                            }
                        }
                        yield break;
                    }
                    else if (protoObj._class.staticClass == null)
                    {
                        dobj = (DynamicObject)((rtObject)protoObj.memberData[0].getValue()).value;
                        var res = getForEach_IEnumerator(player, dobj, frame, step, scope);
                        while (res.MoveNext())
                        {
                            yield return res.Current;
                        }
                    }
                    else
                    {

                        frame.throwError((new error.InternalError(step.token,
                             "遭遇了异常的_prototype_"
                             )));
                        yield break;
                    }
                }
            }
            else if (obj is ASBinCode.rtti.Object)
            {
                var dobj = ((ASBinCode.rtti.DynamicObject)
                    frame.player.static_instance[obj._class.staticClass.classid].value);

                dobj = (ASBinCode.rtti.DynamicObject)((rtObject)dobj.memberData[0].getValue()).value;
                var res = getForEach_IEnumerator(player, dobj, frame, step, scope);
                while (res.MoveNext())
                {
                    yield return res.Current;
                }
            }

            yield break;
        }


    }
}
