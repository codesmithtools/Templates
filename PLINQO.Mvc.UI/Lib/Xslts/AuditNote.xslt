<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:a="http://schemas.codesmithtools.com/datacontext/audit/1.0" xmlns:msxsl="urn:schemas-microsoft-com:xslt" version="1.0" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" />
  <xsl:param name="age" />
  <xsl:template match="/a:audit">
    <xsl:for-each select="a:entity">
      <p>
        <strong>
          <xsl:value-of select="/a:audit/@username" />
        </strong>
        <xsl:choose>
          <xsl:when test="@action='Update'">
            <xsl:text> updated </xsl:text>
          </xsl:when>
          <xsl:when test="@action='Insert'">
            <xsl:text> inserted </xsl:text>
          </xsl:when>
          <xsl:when test="@action='Delete'">
            <xsl:text> deleted </xsl:text>
          </xsl:when>
        </xsl:choose>
        <xsl:value-of select="@type" />
        <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
        <xsl:value-of select="$age" />
        <xsl:text> ago</xsl:text>
        <br />
        <xsl:for-each select="a:property[not(@isForeignKey) or @isForeignKey!='true']">
          <xsl:text disable-output-escaping="yes">&amp;nbsp;&amp;nbsp;&amp;nbsp; </xsl:text>
          <xsl:value-of select="@name" />
          <xsl:choose>
            <xsl:when test="../@action='Update'">
              <xsl:text> changed from </xsl:text>
              <xsl:value-of select="a:original" />
              <xsl:text> to </xsl:text>
              <xsl:value-of select="a:current" />
            </xsl:when>
            <xsl:when test="../@action='Delete'">
              <xsl:text> was </xsl:text>
              <xsl:value-of select="a:original" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:text> is </xsl:text>
              <xsl:value-of select="a:current" />
            </xsl:otherwise>
          </xsl:choose>
          <br />
        </xsl:for-each>
      </p>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>