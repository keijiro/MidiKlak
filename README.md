MidiKlak
========

*MidiKlak* is an extension for [Klak][Klak], that provides functionality for
receiving MIDI messages from physical/virtual MIDI devices.

![gif](http://49.media.tumblr.com/6eb313a0e36f25195e470d59839a530a/tumblr_o1mcacuoft1qio469o1_400.gif)

System Requirements
-------------------

- Unity 5
- Windows or Mac OS X

*MidiKlak* has dependency to the following packages. Please import them before
installing *MidiKlak*.

- [Klak][Klak]
- [MidiJack][MidiJack]

How To Use It
-------------

*MidiKlak* provides only two components -- **NoteInput** and **KnobInput**.

- **NoteInput** - receives MIDI note messages and invokes Unity events
  with input values (note number and velocity).
- **KnobInput** - receives MIDI CC (control change) messages and invokes
  Unity events with a single float value.

For further details of usage, please refer the sample scenes in the "Samples"
directory.

[Klak]: https://github.com/keijiro/Klak
[MidiJack]: https://github.com/keijiro/MidiJack

License
-------

Copyright (C) 2016 Keijiro Takahashi

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
