<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="urn:schemas-microsoft-com:office:spreadsheet"
                xmlns:o="urn:schemas-microsoft-com:office:office"
                xmlns:x="urn:schemas-microsoft-com:office:excel"
                xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
                xmlns:html="http://www.w3.org/TR/REC-html40">
  <xsl:output method="xml" indent="yes" encoding="UTF-8" omit-xml-declaration="yes" />
  <xsl:param name="bankName" />
  <xsl:param name="bankAccountNumber" />
  <xsl:param name="bankEdrpou" />
  <xsl:template match="/">
    <xsl:processing-instruction name="mso-application">
      <xsl:text>progid="Excel.Sheet"</xsl:text>
    </xsl:processing-instruction>
    <Workbook>
      <DocumentProperties xmlns="urn:schemas-microsoft-com:office:office">
        <Author>
          <xsl:apply-templates select="Payment/Account/Person"/>
        </Author>
        <LastAuthor></LastAuthor>
        <LastPrinted></LastPrinted>
        <Created>
          <!--<xsl:value-of select="my:GetCurrentDate()"/>-->
        </Created>
        <Company></Company>
        <Version>12.00</Version>
      </DocumentProperties>
      <ExcelWorkbook xmlns="urn:schemas-microsoft-com:office:excel">
        <WindowHeight>11700</WindowHeight>
        <WindowWidth>28800</WindowWidth>
        <WindowTopX>0</WindowTopX>
        <WindowTopY>0</WindowTopY>
        <ProtectStructure>False</ProtectStructure>
        <ProtectWindows>False</ProtectWindows>
      </ExcelWorkbook>
      <Styles>
        <Style ss:ID="Default" ss:Name="Normal">
          <Alignment ss:Vertical="Bottom"/>
          <Borders/>
          <Font ss:FontName="Calibri" x:Family="Swiss" ss:Size="11" ss:Color="#000000"/>
          <Interior/>
          <NumberFormat/>
          <Protection/>
        </Style>
        <Style ss:ID="s64">
          <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
          <Font ss:FontName="Calibri" x:CharSet="204" x:Family="Swiss" ss:Size="11"
           ss:Color="#000000" ss:Bold="1"/>
        </Style>
        <Style ss:ID="s65">
          <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
          <Font ss:FontName="Calibri" x:CharSet="204" x:Family="Swiss" ss:Size="11"
           ss:Color="#000000" ss:Bold="1"/>          
          <Borders>
            <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
            <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>
            <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>
            <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
          </Borders>
        </Style>
        <Style ss:ID="s66">
          <Borders>
            <Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
            <Border ss:Position="Left" ss:LineStyle="Continuous" ss:Weight="1"/>
            <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>
            <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
          </Borders>
        </Style>
        <Style ss:ID="s77">
          <Borders>
            <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
          </Borders>
        </Style>
        <Style ss:ID="s80">
          <Alignment ss:Horizontal="Center" ss:Vertical="Bottom"/>
          <Borders>
            <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
          </Borders>
          <Font ss:FontName="Calibri" x:CharSet="204" x:Family="Swiss" ss:Size="11"
           ss:Color="#000000" ss:Bold="1"/>
        </Style>
        <Style ss:ID="s81">
          <Borders>
            <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>
          </Borders>
        </Style>
        <Style ss:ID="s82">
          <Borders>
            <Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>
            <Border ss:Position="Top" ss:LineStyle="Continuous" ss:Weight="1"/>
          </Borders>
        </Style>        
      </Styles>
      <xsl:apply-templates select="Payment"/>
      
    </Workbook> 
  </xsl:template>
  
  <xsl:template match="Payment">
    <xsl:variable name="RefCell" select="concat('=R[-',string(9+count(PaymentItems/PaymentItem)),']C')"></xsl:variable>
    <Worksheet>
      <xsl:attribute name="ss:Name">
        <xsl:value-of select="Name"/>
      </xsl:attribute>
      <Table ss:ExpandedColumnCount="1024" ss:ExpandedRowCount="65536"
       x:FullColumns="1" x:FullRows="1" ss:DefaultColumnWidth="54"
       ss:DefaultRowHeight="15">
        <Column ss:AutoFitWidth="0" ss:Width="84.75"/>
        <Column ss:AutoFitWidth="0" ss:Width="5.25"/>
        <Column ss:AutoFitWidth="0" ss:Width="58" ss:Span="1"/>
        <Column ss:AutoFitWidth="0" ss:Width="38.25"/>
        <Column ss:AutoFitWidth="0" ss:Width="38.25"/>
        <Column ss:AutoFitWidth="0" ss:Width="58"/>
        <Column ss:AutoFitWidth="0" ss:Width="58"/>
        <Column ss:AutoFitWidth="0" ss:Width="38.25"/>
        <Column ss:AutoFitWidth="0" ss:Width="51"/>
        <Column ss:AutoFitWidth="0" ss:Width="51"/>
        <Column ss:AutoFitWidth="0" ss:Width="51"/>
        <Row ss:AutoFitHeight="0" ss:Height="16.5">
          <Cell ss:StyleID="s81">
            <Data ss:Type="String">Повiдомлення</Data>
          </Cell>
          <Cell ss:Index="3" ss:MergeAcross="8" ss:StyleID="s64">
            <Data ss:Type="String">СПЛАТА  ЗА  КОМУНАЛЬНI  ПОСЛУГИ</Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3" ss:MergeAcross="8">
            <Data ss:Type="String">
              "<xsl:value-of select="$bankName"/>" рахунок <xsl:value-of select="$bankAccountNumber"/> ЄДРПОУ <xsl:value-of select="$bankEdrpou"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3" ss:MergeAcross="1">
            <Data ss:Type="String">Особистий рахунок</Data>
          </Cell>
          <Cell ss:Index="6" ss:MergeAcross="1" ss:StyleID="s64">
            <Data ss:Type="String">
              <xsl:value-of select="Account/Number"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3" ss:MergeAcross="2">
            <Data ss:Type="String">Прiзвище, iм’я, по батьковi</Data>
          </Cell>
          <Cell ss:MergeAcross="5">
            <Data ss:Type="String">
              <xsl:apply-templates select="Account/Person"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3">
            <Data ss:Type="String">Адреса:</Data>
          </Cell>
          <Cell ss:Index="6" ss:MergeAcross="5">
            <Data ss:Type="String">
              <xsl:apply-templates select="Account"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3" ss:MergeAcross="8">
            <Data ss:Type="String">Пiльга   %</Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="14.0625">
          <Cell ss:StyleID="s81"/>
          <Cell ss:StyleID="s65" ss:Index="3" ss:MergeAcross="1">
            <Data ss:Type="String">Вид платежу</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">C</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">По</Data>
          </Cell>
          <Cell ss:StyleID="s65" ss:MergeAcross="1">
            <Data ss:Type="String">Показання лiчильника</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">Рiзн.</Data>
          </Cell>
          <!--<Cell ss:StyleID="s65"><Data ss:Type="String">Тариф</Data></Cell>-->
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">Сума</Data>
          </Cell>          
        </Row>
        <xsl:apply-templates select="PaymentItems/PaymentItem"></xsl:apply-templates>
        <Row ss:AutoFitHeight="0" ss:Height="15.75">
          <Cell ss:StyleID="s81">
            <Data ss:Type="String">Касир</Data>
          </Cell>
          <Cell ss:Index="3">
            <Data ss:Type="String">Усього</Data>
          </Cell>
          <Cell>
            <Data ss:Type="Number">
              <xsl:value-of select="format-number(sum(PaymentItems/PaymentItem/Amount),'####.##')"/>
            </Data>
          </Cell>
          <Cell ss:Index="6" ss:MergeAcross="5">
            <Data ss:Type="String"> Пiдпис  платника  _________________________</Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="5.25">
          <Cell ss:StyleID="s77"/>
          <Cell ss:StyleID="s77"/>
          <Cell ss:MergeAcross="8" ss:StyleID="s77"/>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="16.5">
          <Cell ss:StyleID="s82"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
          <Cell ss:StyleID="s77"/>
          <Cell ss:Index="3" ss:MergeAcross="8"  ss:StyleID="s80"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3" ss:MergeAcross="8"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3" ss:MergeAcross="1"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
          <Cell ss:Index="6" ss:MergeAcross="1" ss:StyleID="s64"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3" ss:MergeAcross="2"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
          <Cell ss:MergeAcross="5"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
          <Cell ss:Index="6" ss:MergeAcross="5"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:StyleID="s81"/>
          <Cell ss:Index="3" ss:MergeAcross="8"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:StyleID="s81"/>
          <Cell ss:StyleID="s65" ss:Index="3" ss:MergeAcross="1">
            <xsl:attribute name="ss:Formula">
              <xsl:value-of select="$RefCell"/>
            </xsl:attribute>
          </Cell>
          <Cell ss:StyleID="s65">
            <xsl:attribute name="ss:Formula">
              <xsl:value-of select="$RefCell"/>
            </xsl:attribute>
          </Cell>
          <Cell ss:StyleID="s65">
            <xsl:attribute name="ss:Formula">
              <xsl:value-of select="$RefCell"/>
            </xsl:attribute>
          </Cell>

          <Cell ss:StyleID="s65" ss:MergeAcross="1">
            <xsl:attribute name="ss:Formula">
              <xsl:value-of select="$RefCell"/>
            </xsl:attribute>
          </Cell>
          <Cell ss:StyleID="s65">
            <xsl:attribute name="ss:Formula">
              <xsl:value-of select="$RefCell"/>
            </xsl:attribute>
          </Cell>
          <!--<Cell ss:StyleID="s65"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>-->
          <Cell ss:StyleID="s65">
            <xsl:attribute name="ss:Formula">
              <xsl:value-of select="$RefCell"/>
            </xsl:attribute>
          </Cell>          
        </Row>
        <xsl:apply-templates select="PaymentItems/PaymentItem"></xsl:apply-templates>
        <Row ss:AutoFitHeight="0" ss:Height="14.25">
          <Cell ss:StyleID="s81"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
          <Cell ss:Index="3"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
          <Cell><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
          <Cell ss:Index="6" ss:MergeAcross="5"><xsl:attribute name="ss:Formula"><xsl:value-of select="$RefCell"/></xsl:attribute></Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:Index="3" ss:MergeAcross="8"/>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12"/>
        <Row ss:AutoFitHeight="0" ss:Height="12">
          <Cell ss:Index="3" ss:MergeAcross="8"/>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="12" ss:Span="65495"/>        
      </Table>
      <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">
        <PageSetup>
          <Header x:Margin="0.3"/>
          <Footer x:Margin="0.3"/>
          <PageMargins x:Bottom="0.75" x:Left="0.25" x:Right="0.25" x:Top="0.75"/>
        </PageSetup>
        <Unsynced/>
        <Print>
          <ValidPrinterInfo/>
          <PaperSizeIndex>9</PaperSizeIndex>
          <HorizontalResolution>600</HorizontalResolution>
          <VerticalResolution>0</VerticalResolution>
        </Print>
        <Selected/>
        <ProtectObjects>False</ProtectObjects>
        <ProtectScenarios>False</ProtectScenarios>
      </WorksheetOptions>
    </Worksheet>
  </xsl:template>
  <xsl:template match="PaymentItem">
    <Row ss:AutoFitHeight="0" ss:Height="12">
      <Cell ss:StyleID="s81"/>
      <Cell ss:StyleID="s66" ss:Index="3" ss:MergeAcross="1">
        <Data ss:Type="String">
          <xsl:value-of select="Service/Name"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s66">
        <Data ss:Type="String">
          <xsl:value-of select="substring(PeriodFrom,0,8)"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s66">
        <Data ss:Type="String">
          <xsl:value-of select="substring(PeriodTo,0,8)"/>
        </Data>
      </Cell>

      <Cell ss:StyleID="s66">
        <Data ss:Type="Number">
          <xsl:value-of select="LastIndication"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s66">
        <Data ss:Type="Number">
          <xsl:value-of select="CurrentIndication"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s66">
        <Data ss:Type="Number">
          <xsl:value-of select="Value"/>
        </Data>
      </Cell>
      <!--<Cell ss:StyleID="s66">
        <Data ss:Type="Number"></Data>
      </Cell>-->
      <Cell ss:StyleID="s66">
        <Data ss:Type="Number">
          <xsl:value-of select="Amount"/>
        </Data>
      </Cell>      
    </Row>
  </xsl:template>
  <xsl:template match="Person"><xsl:value-of select="concat(LastName,' ',FirstName,' ',SurName)"/></xsl:template>
  <xsl:template match="Account"><xsl:value-of select="concat(City,', ',Street,', ',Building,' / ',Apartment)"/> </xsl:template>
</xsl:stylesheet>
