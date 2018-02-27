/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System.IO;
using NUnit.Framework;
using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;
using OpenFAST.UnitTests.Test;

namespace OpenFAST.UnitTests
{
    [TestFixture]
    public class EncodeDecodeTest
    {
        [Test]
        public void TestComplexMessage()
        {
      

            var template = new MessageTemplate(
                "Company",
                new Field[]
                    {
                        new Scalar("Name", FastType.String, Operator.None, ScalarValue.Undefined, false),
                        new Scalar("Id", FastType.U32, Operator.Increment, ScalarValue.Undefined, false),
                        new Sequence(
                            "Employees",
                            new Field[]
                                {
                                    new Scalar("First Name", FastType.String, Operator.Copy, ScalarValue.Undefined,
                                               false),
                                    new Scalar("Last Name", FastType.String, Operator.Copy, ScalarValue.Undefined, false)
                                    ,
                                    new Scalar("Age", FastType.U32, Operator.Delta, ScalarValue.Undefined, false)
                                }, false),
                        new Group(
                            "Tax Information",
                            new Field[]
                                {
                                    new Scalar("EIN", FastType.String, Operator.None, ScalarValue.Undefined, false)
                                }, false)
                    });

            var aaaInsurance = new Message(template);
            aaaInsurance.SetFieldValue(1, new StringValue("AAA Insurance"));
            aaaInsurance.SetFieldValue(2, new IntegerValue(5));

            var employees = new SequenceValue(template.GetSequence(
                "Employees"));
            employees.Add(new IFieldValue[]
                              {
                                  new StringValue("John"), new StringValue("Doe"),
                                  new IntegerValue(45)
                              });
            employees.Add(new IFieldValue[]
                              {
                                  new StringValue("Jane"), new StringValue("Doe"),
                                  new IntegerValue(48)
                              });
            aaaInsurance.SetFieldValue(3, employees);
            aaaInsurance.SetFieldValue(4,
                                       new GroupValue(template.GetGroup("Tax Information"),
                                                      new IFieldValue[] { new StringValue("99-99999999") }));

            var outStream = new MemoryStream();
            var output = new MessageOutputStream(outStream);
            output.RegisterTemplate(2, template);
            output.WriteMessage(aaaInsurance);

            var abcBuilding = new Message(template);
            abcBuilding.SetFieldValue(1, new StringValue("ABC Building"));
            abcBuilding.SetFieldValue(2, new IntegerValue(6));
            employees = new SequenceValue(template.GetSequence("Employees"));
            employees.Add(new IFieldValue[]
                              {
                                  new StringValue("Bob"), new StringValue("Builder"),
                                  new IntegerValue(3)
                              });
            employees.Add(new IFieldValue[]
                              {
                                  new StringValue("Joe"), new StringValue("Rock"),
                                  new IntegerValue(59)
                              });
            abcBuilding.SetFieldValue(3, employees);
            abcBuilding.SetFieldValue(4,
                                      new GroupValue(template.GetGroup("Tax Information"),
                                                     new IFieldValue[] { new StringValue("99-99999999") }));
            output.WriteMessage(abcBuilding);

            var input = new MessageInputStream(new MemoryStream(outStream.ToArray()));
            input.RegisterTemplate(2, template);

            GroupValue message = input.ReadMessage();
            Assert.AreEqual(aaaInsurance, message);

            message = input.ReadMessage();
            Assert.AreEqual(abcBuilding, message);
        }

        [Test]
        public void TestMultipleMessages()
        {
            var outStream = new MemoryStream();
            var output = new MessageOutputStream(outStream);
            output.RegisterTemplate(ObjectMother.AllocInstrctnTemplateId,
                                    ObjectMother.AllocationInstruction);

            var allocations = new SequenceValue(ObjectMother.AllocationInstruction
                                                    .GetSequence("Allocations"));
            allocations.Add(ObjectMother.NewAllocation("fortyFiveFund", 22.5, 75.0));
            allocations.Add(ObjectMother.NewAllocation("fortyFund", 24.6, 25.0));

            Message ai1 = ObjectMother.NewAllocInstrctn(
                "ltg0001", 1, 100.0, 23.4, ObjectMother.NewInstrument("CTYA", "200910"), allocations);

            allocations = new SequenceValue(
                ObjectMother.AllocationInstruction.GetSequence("Allocations"));

            allocations.Add(ObjectMother.NewAllocation("fortyFiveFund", 22.5, 75.0));
            allocations.Add(ObjectMother.NewAllocation("fortyFund", 24.6, 25.0));

            Message ai2 = ObjectMother.NewAllocInstrctn(
                "ltg0001", 1, 100.0, 23.4, ObjectMother.NewInstrument("CTYA", "200910"), allocations);

            allocations = new SequenceValue(
                ObjectMother.AllocationInstruction.GetSequence("Allocations"));
            allocations.Add(ObjectMother.NewAllocation("fortyFiveFund", 22.5, 75.0));
            allocations.Add(ObjectMother.NewAllocation("fortyFund", 24.6, 25.0));

            Message ai3 = ObjectMother.NewAllocInstrctn(
                "ltg0001", 1, 100.0, 23.4, ObjectMother.NewInstrument("CTYA", "200910"), allocations);

            output.WriteMessage(ai1);
            output.WriteMessage(ai2);
            output.WriteMessage(ai3);

            byte[] bytes = outStream.ToArray();
            var input = new MessageInputStream(new MemoryStream(bytes));
            input.RegisterTemplate(ObjectMother.AllocInstrctnTemplateId,
                                   ObjectMother.AllocationInstruction);

            Message message = input.ReadMessage();
            Assert.AreEqual(ai1, message);
            message = input.ReadMessage();
            Assert.AreEqual(ai2, message);
            Assert.AreEqual(ai3, input.ReadMessage());
        }
    }
}