using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using STM32Cube_EmBlocks.Properties;


namespace STM32Cube_EmBlocks
{
  public partial class Form1 : Form
  {
    private string mcu = "";
    private string mcuVariant = "";
    private string STM32Series="F4";
    private string CortexType = "M4";

    public Form1()
    {
      InitializeComponent();
      tbProjectDirectory.Text = Settings.Default.ProjectDirectory;
    }


    public static XmlWriter xw = null;


    public static void writeXML(string[] strs, Boolean brk)
    {
      xw.WriteStartElement(strs[0]);
      for (int i = 1; i < strs.Count(); i += 2)
        xw.WriteAttributeString(strs[i], strs[i + 1]);
      if (brk)
        xw.WriteEndElement();
    }


    public static bool ReadSrcFilePaths(string prj, out string result)
    {
      StreamReader _Sr;
      String _Input = "";
      string _HeaderPath = "";
      result = "";

      try
      {
        _Sr = File.OpenText(prj);
      }
      catch (Exception _Ex)
      {
        result = _Ex.ToString();
        return false;
      }

      while ((_Input = _Sr.ReadLine()) != null)
      {
        if (_Input.Contains("HeaderPath="))
        {          
//            _HeaderPath = _Input.Substring(11).Replace("/", @"\");
          _HeaderPath = "Inc";
        }

        if (_Input.Contains("HeaderFiles=") && _HeaderPath != "")
        {
          string[] Headerfiles;
          Headerfiles = _Input.Substring(12).Replace("/", @"\").Split(';');
          foreach (string _Headerfile in Headerfiles)
            if (_Headerfile.Trim()!="")
              writeXML(new string[] {"Unit", "filename", _HeaderPath + @"\" + _Headerfile}, true);
        }

        if (_Input.Contains("[PreviousUsedTStudioFiles]"))
        {
          _Input = _Sr.ReadLine();
          if (_Input.Contains("HeaderPath="))
          {
            _Input = _Sr.ReadLine();
            if (_Input.Contains("SourceFiles="))
            {
              int j, i = 11;

              while (i < _Input.Length)
              {
                j = i;
                i = _Input.IndexOf(';', i + 1);
                if (i > 0)
                {
                  string fn = _Input.Substring(j, i - j).Substring(7).Replace("/", @"\").ToLower();

                  writeXML(new string[] {"Unit", "filename", fn}, false);
                  if (fn[fn.Length - 1] == 's')
                    writeXML(new string[] {"Option", "compilerVar", "ASM"}, true);
                  else
                    writeXML(new string[] {"Option", "compilerVar", "CC"}, true);
                  xw.WriteEndElement();
                }
                else
                  return true;
              }
              return true;
            }
          }
          else if (_Input.Contains("SourceFiles="))
          {
            int j, i = 0;
            while (i < _Input.Length)
            {
              j = i;
              i = _Input.IndexOf(';', i + 1);
              if (i > 0)
              {
                string fn = _Input.Substring(j, i - j).Substring(5).Replace("/", @"\").ToLower();
                writeXML(new string[] {"Unit", "filename", fn}, false);
                if (fn[fn.Length - 1] == 's')
                  writeXML(new string[] {"Option", "compilerVar", "ASM"}, true);
                else
                  writeXML(new string[] {"Option", "compilerVar", "CC"}, true);
                xw.WriteEndElement();
              }
              else
                return true;
            }
            return true;
          }
          else
          {
            result = "ERROR: No Source file found!";
            return false;
          }
        }
      }
      result = "ERROR: No Source file found!";
      return false;
    }


    private bool AddIncludeDirectories(string prj, out string result)
    {
      StreamReader _Sr;
      String _Input = "";
      bool UsesMiddleWare = false;
      result = "";
/*
      //                    writeXML(new string[] { "Add", "directory", @".\Drivers\CMSIS\Device\ST\STM32F4xx\Include" }, true);
      writeXML(new string[] {"Add", "directory", @"Drivers\CMSIS\Device\ST\" + mcu.Substring(0, mcu.Length - 4) + "XX" + @"\Include"},
               true);
      writeXML(new string[] {"Add", "directory", @"Drivers\CMSIS\Include"}, true);
      //                    writeXML(new string[] { "Add", "directory", @".\Drivers\STM32F4xx_HAL_Driver\Inc" }, true);
      writeXML(new string[] {"Add", "directory", @"Drivers\" + mcu.Substring(0, mcu.Length - 4) + "XX" + @"_HAL_Driver\Inc"}, true);*/
      writeXML(new string[] { "Add", "directory", @"." }, true);
      writeXML(new string[] { "Add", "directory", @"Inc" }, true);

      try
      {
        _Sr = File.OpenText(prj);
      }
      catch (Exception _Ex)
      {
        result = _Ex.ToString();
        return false;
      }

      while ((_Input = _Sr.ReadLine()) != null)
      {
        if (_Input.Contains("[PreviousUsedTStudioFiles]"))
        {
          _Input = _Sr.ReadLine();

          if (_Input.Contains("HeaderPath="))
          {
            string[] _HeaderPaths = _Input.Substring(11).Replace("/", @"\").Split(';');
            foreach (string _HeaderPath in _HeaderPaths)
            {
              string _Path = _HeaderPath;
              if (_Path.StartsWith(@"..\..\") == true)
                _Path = _Path.Substring(6).Trim();

              if (_Path.Length > 0)
              {
                if (UsesMiddleWare == false && _Path.StartsWith("Middlewares"))
                {
                  writeXML(new string[] { "Add", "directory", @"Middlewares\ST\STM32_USB_Device_Library\Core\Inc" }, true);
                  UsesMiddleWare = true;
                }     
                writeXML(new string[] { "Add", "directory", _Path }, true);
              }

            }
            return true;
          }
        }
      }
      result = "Couldn't find header paths";
      return true;
    }


    private bool AddHeaderFiles(string prj, out string result)
    {
      StreamReader _Sr;
      String _Input = "";
      result = "";

      try
      {
        _Sr = File.OpenText(prj);
      }
      catch (Exception _Ex)
      {
        result = _Ex.ToString();
        return false;
      }

      while ((_Input = _Sr.ReadLine()) != null)
      {
        if (_Input.Contains("[PreviousLibFiles]"))
        {
          _Input = _Sr.ReadLine();

          if (_Input.Contains("LibFiles="))
          {
            string[] _HeaderFiles = _Input.Substring(9).Replace("/", @"\").Split(';');
            foreach (string _HeaderFile in _HeaderFiles)
            {
              if (_HeaderFile.ToLower().EndsWith(".h"))
                writeXML(new string[] { "Unit", "filename", _HeaderFile }, true);
            }
            return true;
          }
        }
      }
      result = "Couldn't find header files";
      return false;
    }


    private void AddLinkerOptions()
    {
      xw.WriteStartElement("Linker");
        writeXML(new string[] {"Add", "option", "-Wl,--gc-sections"}, true);
        writeXML(new string[] { "Add", "option", "-Wl,--allow-multiple-definition" }, true);
      xw.WriteEndElement();
    }


    private void btnConvertProject_Click(object sender, EventArgs e)
    {
        string pt = Settings.Default.ProjectDirectory;
        string fn = Path.GetFileName(pt);
        string[] ldf = Directory.GetFiles(pt + @"\Projects\TrueSTUDIO\" + fn + " Configuration", "*.ld");
        string Result;
        
        //Get the mcu type from the linkerscriptfilename 
        mcu = ldf[0].Substring(ldf[0].LastIndexOf("\\"));
        int pos = mcu.IndexOf("_", 0);
        mcu = mcu.Substring(1, pos - 1);
        mcuVariant = mcu.Substring(mcu.Length - 1, 1);
        string STM32Series = mcu.ToUpper().Replace("STM32","").Substring(0,2);

        switch (STM32Series)
        {
          case "F0":
            CortexType = "M0";
            break;
          case "L0":
            CortexType = "M0+";
            break;

          case "F1":
          case "F2":
          case "L1":
            CortexType = "M3";
            break;
          
          case "F3":
          case "F4":
          case "L4":
            CortexType = "M4";
            break;

          case "F7":
            CortexType = "M7";
            break;
        }

        //Added by EvertDekker 20140905

        foreach (string f in ldf)
          File.Copy(f, pt + "\\" + Path.GetFileName(f), true);

        try
        {
          XmlWriterSettings settings = new XmlWriterSettings();
          settings.Indent = true;
          settings.IndentChars = ("\t");
          xw = XmlWriter.Create(pt + "\\" + fn + ".ebp", settings);
          xw.WriteStartDocument(true);
          xw.WriteStartElement("EmBlocks_project_file");
          writeXML(new string[] {"EmBlocksVersion", "release", "2.10", "revision", "1"}, true);
          writeXML(new string[] {"FileVersion", "major", "1", "minor", "0"}, true);

          xw.WriteStartElement("Project");
            writeXML(new string[] {"Option", "title", fn}, true);
            writeXML(new string[] {"Option", "pch_mode", "2"}, true);
            writeXML(new string[] {"Option", "compiler", "armgcc_eb"}, true);

// Builds
            xw.WriteStartElement("Build");
              writeXML(new string[] {"Target", "title", "Debug"}, false);
// Builds/DEBUG  //
                writeXML(new string[] {"Option", "output", @".\Debug\" + fn + ".elf"}, true);
                writeXML(new string[] {"Option", "object_output", @".\Debug"}, true);
                writeXML(new string[] {"Option", "type", "0"}, true);
                writeXML(new string[] {"Option", "compiler", "armgcc_eb"}, true);
                writeXML(new string[] {"Option", "projectDeviceOptionsRelation", "0"}, true);

                xw.WriteStartElement("Compiler");
                  writeXML(new string[] {"Add", "option", "-Wall"}, true);
                  writeXML(new string[] {"Add", "option", "-fdata-sections"}, true);
                  writeXML(new string[] {"Add", "option", "-ffunction-sections"}, true);
                  writeXML(new string[] {"Add", "option", "-O0"}, true);
                  writeXML(new string[] {"Add", "option", "-g3"}, true);
                  writeXML(new string[] {"Add", "option", "-DUSE_HAL_DRIVER"}, true);
                  //writeXML(new string[] { "Add", "option", "-DSTM32F407xx" }, true);
                  writeXML(new string[] { "Add", "option", "-D" + mcu + "xx" }, true);
                  writeXML(new string[] { "Add", "option", "-D" + mcu.Substring(0, mcu.Length - 2) + "x" + mcuVariant}, true);

                  if(AddIncludeDirectories(pt + "\\.mxproject", out Result)==false)
                    MessageBox.Show(Result); 
                xw.WriteEndElement();

                xw.WriteStartElement("Assembler");
                  writeXML(new string[] {"Add", "option", "-Wa,--gdwarf-2"}, true);
                xw.WriteEndElement();

                AddLinkerOptions();
              xw.WriteEndElement();

              writeXML(new string[] {"Target", "title", "Release"}, false);
// Builds/RELEASE //
                writeXML(new string[] {"Option", "output", @".\Release\" + fn + ".elf"}, true);
                writeXML(new string[] {"Option", "object_output", @".\Release"}, true);
                writeXML(new string[] {"Option", "type", "0"}, true);
                writeXML(new string[] {"Option", "create_hex", "1"}, true);
                writeXML(new string[] {"Option", "compiler", "armgcc_eb"}, true);
                writeXML(new string[] {"Option", "projectDeviceOptionsRelation", "0"}, true);

                xw.WriteStartElement("Compiler");
                writeXML(new string[] {"Add", "option", "-fdata-sections"}, true);
                writeXML(new string[] {"Add", "option", "-ffunction-sections"}, true);
                writeXML(new string[] {"Add", "option", "-O2"}, true);
                writeXML(new string[] {"Add", "option", "-g2"}, true);
                writeXML(new string[] {"Add", "option", "-DUSE_HAL_DRIVER"}, true);
                //writeXML(new string[] { "Add", "option", "-DSTM32F407xx" }, true);
                writeXML(new string[] {"Add", "option", "-D" + mcu + "xx"}, true);

                if (AddIncludeDirectories(pt + "\\.mxproject", out Result) == false)
                  MessageBox.Show(Result);

                xw.WriteEndElement();

                xw.WriteStartElement("Assembler");
                  writeXML(new string[] {"Add", "option", "-Wa,--no-warn"}, true);
                xw.WriteEndElement();

                AddLinkerOptions();

              xw.WriteEndElement();
            xw.WriteEndElement();

            xw.WriteStartElement("Device");
              writeXML(new string[] { "Add", "option", "$device=cortex-" + CortexType.ToLower() }, true);
              if (CortexType == "M4" || CortexType == "M7")
                writeXML(new string[] {"Add", "option", "$fpu=fpv4-sp-d16"}, true);
              // writeXML(new string[] { "Add", "option", @"$lscript=.\STM32F407VG_FLASH.ld" }, true);
              writeXML(new string[] {"Add", "option", @"$lscript=.\" + mcu + "_FLASH.ld"}, true);
              writeXML(new string[] {"Add", "option", "$stack=0x0100"}, true);
              writeXML(new string[] {"Add", "option", "$heap=0x0000"}, true);
            xw.WriteEndElement();

            xw.WriteStartElement("Compiler");
              if (CortexType == "M4" || CortexType == "M7")
              {
                writeXML(new string[] {"Add", "option", "-mfloat-abi=hard"}, true);
                writeXML(new string[] { "Add", "option", "-DARM_MATH_C" + CortexType }, true);
                writeXML(new string[] {"Add", "option", "-D__FPU_USED"}, true);
              }
              else
              {
                writeXML(new string[] {"Add", "option", "-mfloat-abi=soft"}, true); 
              }
              //writeXML(new string[] { "Add", "option", "-DSTM32F407VG" }, true);
              writeXML(new string[] {"Add", "option", "-D" + mcu.Substring(0, mcu.Length - 2) + "xx"}, true); //+ mcuVariant 
              writeXML(new string[] {"Add", "option", "-DUSE_STDPERIPH_DRIVER"}, true);
              writeXML(new string[] {"Add", "option", "-fno-strict-aliasing"}, true);

              if (AddIncludeDirectories(pt + "\\.mxproject", out Result) == false)
                MessageBox.Show(Result);
            xw.WriteEndElement();

            xw.WriteStartElement("Linker");
              writeXML(new string[] {"Add", "option", "-eb_start_files"}, true);
              writeXML(new string[] {"Add", "option", "-eb_lib=n"}, true);
            xw.WriteEndElement();

            if (ReadSrcFilePaths(pt + "\\.mxproject", out Result) == false)
              MessageBox.Show(Result);

            if (AddHeaderFiles(pt + "\\.mxproject", out Result) == false)
              MessageBox.Show(Result);

            xw.WriteStartElement("Extensions");
              xw.WriteStartElement("code_completion");
              xw.WriteEndElement();

              xw.WriteStartElement("debugger");
                writeXML(new string[] {"target_debugging_settings", "target", "Debug", "active_interface", "ST-link"}, false);
                  writeXML(
                             new string[]
                             {
                               "debug_interface", "interface_id", "ST-link", "ip_address", "localhost", "ip_port", "4242", "path",
                               @"${EMBLOCKS}\share\contrib", "executable", "STLinkGDB.exe", "description", "", "dont_start_server", "false",
                               "backoff_time", "1000", "options", "6", "active_family", "STMicroelectronics"
                             }, false);
                      writeXML(new string[] {"family_options", "family_id", "STMicroelectronics"}, false);
                        writeXML(new string[] {"option", "opt_id", "ID_JTAG_SWD", "opt_value", "swd"}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_VECTOR_START", "opt_value", "0x08000000"}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_RESET_TYPE", "opt_value", "System"}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_LOAD_PROGRAM", "opt_value", "1"}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_SEMIHOST_CHECK", "opt_value", "0"}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_RAM_EXEC", "opt_value", "0"}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_VEC_TABLE", "opt_value", "1"}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_DONT_CONN_RESET", "opt_value", "0"}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_ALL_MODE_DEBUG", "opt_value", "0"}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_DEV_ADDR", "opt_value", ""}, true);
                        writeXML(new string[] {"option", "opt_id", "ID_VERBOSE_LEVEL", "opt_value", "3"}, true);
                      xw.WriteEndElement(); // family_options
                  xw.WriteEndElement(); // debug_interface
                xw.WriteEndElement(); // target_debugging_settings
              xw.WriteEndElement(); // debugger
            xw.WriteStartElement("envvars");

          xw.WriteEndDocument();  // Extensions/Project/EmBlocks_project_file
          xw.Flush();
          MessageBox.Show("Convertion done!");
        }
        finally
        {
          if (xw != null)
            xw.Close();
        }
      }

    private void btnSelectFolder_Click_1(object sender, EventArgs e)
    {
      folderBrowserDialog1.SelectedPath = @"C:\work\Misc routines\F4_Maxed_Out";
      if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
      {
        Settings.Default.ProjectDirectory = folderBrowserDialog1.SelectedPath;
        Settings.Default.Save();
        tbProjectDirectory.Text = Settings.Default.ProjectDirectory;
      }    
    }

  }
}
