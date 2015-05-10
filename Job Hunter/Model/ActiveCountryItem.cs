/*
 * Copyright (c) 2015 Rafael Baquero
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Job_Hunter.Model
{
    public class ActiveCountryItem
    {
        protected long _id;
        protected string _iso2Code;
        protected string _note;

        public ActiveCountryItem(long id, string iso2Code, string note)
        {
            _id = id;
            _iso2Code = iso2Code;
            _note = note;
        }

        public long Id
        {
            get { return _id; }
        }

        public string Iso2Code
        {
            get { return _iso2Code; }
        }

        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }
    }
}
