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
    public class StatusItem
    {
        private long _id;
        private long _position;
        private string _status;
        private string _note;

        public StatusItem(long id, long position, string status, string note)
        {
            _id = id;
            _position = position;
            _status = status;
            _note = note;
        }

        public long Id
        {
            get { return _id; }
        }

        public long Position
        {
            get { return _position; }
        }

        public string Status
        {
            get { return _status; }
        }

        public string Note
        {
            get { return _note; }
        }
    }
}
