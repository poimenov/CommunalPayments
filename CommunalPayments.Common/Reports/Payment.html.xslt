<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns="http://www.w3.org/1999/xhtml"
    xmlns:settings="urn:CommunalPayments.WPF.Properties.Settings">
  <xsl:output method="html" indent="yes" omit-xml-declaration="yes"/>
  <xsl:param name="bankName" />
  <xsl:param name="bankAccountNumber" />
  <xsl:param name="bankEdrpou" />

  <xsl:template match="/">
    <html>
      <head>
        <title>Оплата коммунальных услуг</title>
        <style>
          body{
          padding: 10px;
          width:800px;
          }
          div{
          font-family: Helvetica,Arial,Sans-serif;
          font-size: 12pt;
          padding-bottom: 5px;
          }
          .order-title{
          font-size: 13pt;
          font-weight: bold;
          text-align:center;
          padding-bottom: 15px;
          }
          table{
          border-collapse:collapse;
          width:100%;
          font-family: Helvetica,Arial,Sans-serif;
          font-size: 12pt;
          margin: 10px 0px 10px 0px;
          }
          th,td{
          border: 1px solid gray;
          padding: 2px;
          }
          .my{
          text-align:center;
          }
          .dc{
          text-align:right;
          }
        </style>
      </head>
      <xsl:apply-templates select="Payment"/>
    </html>
  </xsl:template>
  <xsl:template match="Payment">
    <body>
      <div>
        <div class="order-title">
          Оплата коммунальных услуг<br/>
          "<xsl:value-of select="$bankName"/>" рахунок <xsl:value-of select="$bankAccountNumber"/> ЄДРПОУ <xsl:value-of select="$bankEdrpou"/>
        </div>
        <div>
          Личный счет: <b>
            <xsl:value-of select="Account/Number"/>
          </b>
        </div>
        <xsl:apply-templates select="Account/Person"/>
        <xsl:apply-templates select="Account"/>
      </div>
      <table cellpadding="2" cellspacing="0">
        <thead>
          <tr>
            <th rowspan="2">Услуга</th>
            <th colspan="2">Период</th>
            <th colspan="3">Показания счетчика</th>
            <th rowspan="2">Сумма</th>
          </tr>
          <tr>
            <th>с</th>
            <th>до</th>
            <th>пред.</th>
            <th>текущее</th>
            <th>разница</th>
          </tr>
        </thead>
        <tbody>
          <xsl:apply-templates select="PaymentItems/PaymentItem">
            <xsl:sort select="Service/ErcId" data-type="number" order="ascending"/>
          </xsl:apply-templates>
        </tbody>
      </table>
      <div>
        Итого: <b>
          <xsl:value-of select="format-number(sum(PaymentItems/PaymentItem/Amount),'#.##')"/> грн.
        </b>
      </div>
    </body>
  </xsl:template>
  <xsl:template match="Person">
    <div>
      Плательщик: <xsl:value-of select="concat(LastName,' ',FirstName,' ',SurName)"/>
    </div>
  </xsl:template>
  <xsl:template match="Account">
    <div>
      Адрес: <xsl:value-of select="concat(City,', ',Street,', ',Building,' / ',Apartment)"/>
    </div>
  </xsl:template>
  <xsl:template match="PaymentItem">
    <tr>
      <td>
        <xsl:value-of select="Service/Name"/>
      </td>
      <td class="my">
        <xsl:value-of select="substring(PeriodFrom,0,8)"/>
      </td>
      <td class="my">
        <xsl:value-of select="substring(PeriodTo,0,8)"/>
      </td>
      <td class="dc">
        <xsl:value-of select="LastIndication"/>
      </td>
      <td class="dc">
        <xsl:value-of select="CurrentIndication"/>
      </td>
      <td class="dc">
        <xsl:value-of select="Value"/>
      </td>
      <td class="dc">
        <xsl:value-of select="Amount"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
