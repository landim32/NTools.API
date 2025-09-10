import { useState } from "react"
import { Prism as SyntaxHighlighter } from 'react-syntax-highlighter'
import { oneDark, oneLight } from 'react-syntax-highlighter/dist/esm/styles/prism'
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Copy, Check } from "lucide-react"
import { useTheme } from "@/hooks/use-theme"
import { useToast } from "@/hooks/use-toast"

interface CodeExampleProps {
  reactCode: string
  dotnetCode: string
  title?: string
}

export function CodeExample({ reactCode, dotnetCode, title = "Implementation" }: CodeExampleProps) {
  const [copiedCode, setCopiedCode] = useState<string | null>(null)
  const { theme } = useTheme()
  const { toast } = useToast()

  const copyToClipboard = async (code: string, type: string) => {
    try {
      await navigator.clipboard.writeText(code)
      setCopiedCode(type)
      toast({
        description: `${type} code copied to clipboard!`,
      })
      setTimeout(() => setCopiedCode(null), 2000)
    } catch (err) {
      toast({
        variant: "destructive",
        description: "Failed to copy code to clipboard",
      })
    }
  }

  const codeStyle = theme === 'dark' ? oneDark : oneLight

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle className="text-lg font-semibold">{title}</CardTitle>
      </CardHeader>
      <CardContent>
        <Tabs defaultValue="react" className="w-full">
          <TabsList className="grid w-full grid-cols-2">
            <TabsTrigger value="react">React</TabsTrigger>
            <TabsTrigger value="dotnet">.NET Core</TabsTrigger>
          </TabsList>
          
          <TabsContent value="react" className="space-y-2">
            <div className="flex items-center justify-between">
              <h4 className="text-sm font-medium">Frontend Implementation</h4>
              <Button
                variant="outline"
                size="sm"
                onClick={() => copyToClipboard(reactCode, 'React')}
                className="h-8"
              >
                {copiedCode === 'React' ? (
                  <Check className="h-3 w-3" />
                ) : (
                  <Copy className="h-3 w-3" />
                )}
              </Button>
            </div>
            <div className="rounded-md overflow-hidden border border-border">
              <SyntaxHighlighter
                language="typescript"
                style={codeStyle}
                customStyle={{
                  margin: 0,
                  borderRadius: 0,
                  fontSize: '0.875rem'
                }}
              >
                {reactCode}
              </SyntaxHighlighter>
            </div>
          </TabsContent>
          
          <TabsContent value="dotnet" className="space-y-2">
            <div className="flex items-center justify-between">
              <h4 className="text-sm font-medium">Backend API Implementation</h4>
              <Button
                variant="outline"
                size="sm"
                onClick={() => copyToClipboard(dotnetCode, '.NET')}
                className="h-8"
              >
                {copiedCode === '.NET' ? (
                  <Check className="h-3 w-3" />
                ) : (
                  <Copy className="h-3 w-3" />
                )}
              </Button>
            </div>
            <div className="rounded-md overflow-hidden border border-border">
              <SyntaxHighlighter
                language="csharp"
                style={codeStyle}
                customStyle={{
                  margin: 0,
                  borderRadius: 0,
                  fontSize: '0.875rem'
                }}
              >
                {dotnetCode}
              </SyntaxHighlighter>
            </div>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  )
}