<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
   xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
   xmlns:fo="http://www.w3.org/1999/XSL/Format"
   xmlns:settings="urn:CommunalPayments.WPF.Properties.Settings">
  <xsl:output method="xml" indent="yes" encoding="UTF-8" omit-xml-declaration="no" />
  <xsl:param name="bankName" />
  <xsl:param name="bankAccountNumber" />
  <xsl:param name="bankEdrpou" />
  <xsl:variable name="font-family">Tahoma</xsl:variable>
  <xsl:template match="/">
    <fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format" language="RU">
      <fo:layout-master-set>
        <fo:simple-page-master master-name="simpleA4"
          page-height="29.7cm" page-width="21cm"
          margin-top="5mm" margin-bottom="5mm" margin-left="2cm" margin-right="5mm">
          <fo:region-body/>
        </fo:simple-page-master>
      </fo:layout-master-set>
      <fo:page-sequence master-reference="simpleA4">
        <fo:flow flow-name="xsl-region-body">
          <xsl:apply-templates select="Payment"/>
        </fo:flow>
      </fo:page-sequence>
    </fo:root>
  </xsl:template>

  <xsl:template match="Payment">
    <fo:block  font-size="13pt" font-weight="bold" text-align="center">
      <xsl:attribute name="font-family">
        <xsl:value-of select="$font-family"/>
      </xsl:attribute>
      Оплата коммунальных услуг
    </fo:block>
    <fo:block font-size="12pt" text-align="center" space-after="3mm">
      <xsl:attribute name="font-family">
        <xsl:value-of select="$font-family"/>
      </xsl:attribute>
      "<xsl:value-of select="$bankName"/>" рахунок <xsl:value-of select="$bankAccountNumber"/> ЄДРПОУ <xsl:value-of select="$bankEdrpou"/>
    </fo:block>
    <fo:block font-size="12pt">
      <xsl:attribute name="font-family">
        <xsl:value-of select="$font-family"/>
      </xsl:attribute>
      Личный счет: <xsl:value-of select="Account/Number"/>
    </fo:block>
    <xsl:apply-templates select="Account/Person"/>
    <xsl:apply-templates select="Account"/>
    <!-- table-->

    <fo:table table-layout="fixed" width="180mm" border-collapse="collapse" space-before="3mm" space-after="3mm">
      <fo:table-column column-width="45mm" />
      <fo:table-column column-width="20mm" />
      <fo:table-column column-width="20mm" />
      <fo:table-column column-width="25mm" />
      <fo:table-column column-width="25mm" />
      <fo:table-column column-width="25mm" />
      <fo:table-column column-width="20mm" />
      <fo:table-header>
        <fo:table-row>
          <fo:table-cell number-rows-spanned="2" border="solid thin" text-align="center">
            <fo:block font-size="12pt" font-weight="bold">
              <xsl:attribute name="font-family">
                <xsl:value-of select="$font-family"/>
              </xsl:attribute>Услуга
            </fo:block>
          </fo:table-cell>
          <fo:table-cell number-columns-spanned="2" border="solid thin" text-align="center">
            <fo:block font-size="12pt" font-weight="bold">
              <xsl:attribute name="font-family">
                <xsl:value-of select="$font-family"/>
              </xsl:attribute>Период
            </fo:block>
          </fo:table-cell>
          <fo:table-cell number-columns-spanned="3" border="solid thin" text-align="center">
            <fo:block font-size="12pt" font-weight="bold">
              <xsl:attribute name="font-family">
                <xsl:value-of select="$font-family"/>
              </xsl:attribute>Показания счетчика
            </fo:block>
          </fo:table-cell>
          <fo:table-cell number-rows-spanned="2" border="solid thin" text-align="center">
            <fo:block font-size="12pt" font-weight="bold">
              <xsl:attribute name="font-family">
                <xsl:value-of select="$font-family"/>
              </xsl:attribute>Сумма
            </fo:block>
          </fo:table-cell>
        </fo:table-row>
        <fo:table-row>
          <fo:table-cell border="solid thin" text-align="center">
            <fo:block font-size="12pt" font-weight="bold">
              <xsl:attribute name="font-family">
                <xsl:value-of select="$font-family"/>
              </xsl:attribute>с
            </fo:block>
          </fo:table-cell>
          <fo:table-cell border="solid thin" text-align="center">
            <fo:block font-size="12pt" font-weight="bold">
              <xsl:attribute name="font-family">
                <xsl:value-of select="$font-family"/>
              </xsl:attribute>до
            </fo:block>
          </fo:table-cell>
          <fo:table-cell border="solid thin" text-align="center">
            <fo:block font-size="12pt" font-weight="bold">
              <xsl:attribute name="font-family">
                <xsl:value-of select="$font-family"/>
              </xsl:attribute>пред.
            </fo:block>
          </fo:table-cell>
          <fo:table-cell border="solid thin" text-align="center">
            <fo:block font-size="12pt" font-weight="bold">
              <xsl:attribute name="font-family">
                <xsl:value-of select="$font-family"/>
              </xsl:attribute>текущее
            </fo:block>
          </fo:table-cell>
          <fo:table-cell border="solid thin" text-align="center">
            <fo:block font-size="12pt" font-weight="bold">
              <xsl:attribute name="font-family">
                <xsl:value-of select="$font-family"/>
              </xsl:attribute>разница
            </fo:block>
          </fo:table-cell>
        </fo:table-row>
      </fo:table-header>
      <fo:table-body>
        <xsl:apply-templates select="PaymentItems/PaymentItem">
          <xsl:sort select="Service/ErcId" data-type="number" order="ascending"/>
        </xsl:apply-templates>
      </fo:table-body>
    </fo:table>

    <fo:block font-size="12pt">
      <xsl:attribute name="font-family">
        <xsl:value-of select="$font-family"/>
      </xsl:attribute>
      Итого: <xsl:value-of select="format-number(sum(PaymentItems/PaymentItem/Amount),'#.##')"/> грн.
    </fo:block>

  </xsl:template>

  <xsl:template match="Person">
    <fo:block font-size="12pt">
      <xsl:attribute name="font-family">
        <xsl:value-of select="$font-family"/>
      </xsl:attribute>
      Плательщик: <xsl:value-of select="concat(LastName,' ',FirstName,' ',SurName)"/>
    </fo:block>
  </xsl:template>

  <xsl:template match="Account">
    <fo:block font-size="12pt">
      <xsl:attribute name="font-family">
        <xsl:value-of select="$font-family"/>
      </xsl:attribute>
      Адрес: <xsl:value-of select="concat(City,', ',Street,', ',Building,' / ',Apartment)"/>
    </fo:block>
  </xsl:template>

  <xsl:template match="PaymentItem">
    <fo:table-row>
      <fo:table-cell border="solid thin">
        <fo:block font-size="12pt">
          <xsl:attribute name="font-family">
            <xsl:value-of select="$font-family"/>
          </xsl:attribute>
          <xsl:value-of select="Service/Name"/>
        </fo:block>
      </fo:table-cell>
      <fo:table-cell text-align="center" border="solid thin">
        <fo:block font-size="12pt">
          <xsl:attribute name="font-family">
            <xsl:value-of select="$font-family"/>
          </xsl:attribute>
          <xsl:value-of select="substring(PeriodFrom,0,8)"/>
        </fo:block>
      </fo:table-cell>
      <fo:table-cell text-align="center" border="solid thin">
        <fo:block font-size="12pt">
          <xsl:attribute name="font-family">
            <xsl:value-of select="$font-family"/>
          </xsl:attribute>
          <xsl:value-of select="substring(PeriodTo,0,8)"/>
        </fo:block>
      </fo:table-cell>
      <fo:table-cell text-align="right" border="solid thin">
        <fo:block font-size="12pt">
          <xsl:attribute name="font-family">
            <xsl:value-of select="$font-family"/>
          </xsl:attribute>
          <xsl:value-of select="LastIndication"/>
          <xsl:text> </xsl:text>
        </fo:block>
      </fo:table-cell>
      <fo:table-cell text-align="right" border="solid thin">
        <fo:block font-size="12pt">
          <xsl:attribute name="font-family">
            <xsl:value-of select="$font-family"/>
          </xsl:attribute>
          <xsl:value-of select="CurrentIndication"/>
          <xsl:text> </xsl:text>
        </fo:block>
      </fo:table-cell>
      <fo:table-cell text-align="right" border="solid thin">
        <fo:block font-size="12pt">
          <xsl:attribute name="font-family">
            <xsl:value-of select="$font-family"/>
          </xsl:attribute>
          <xsl:value-of select="Value"/>
          <xsl:text> </xsl:text>
        </fo:block>
      </fo:table-cell>
      <fo:table-cell text-align="right" border="solid thin">
        <fo:block font-size="12pt">
          <xsl:attribute name="font-family">
            <xsl:value-of select="$font-family"/>
          </xsl:attribute>
          <xsl:value-of select="Amount"/>
          <xsl:text> </xsl:text>
        </fo:block>
      </fo:table-cell>
    </fo:table-row>
  </xsl:template>

</xsl:stylesheet>
