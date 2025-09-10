import { Github, BookOpen, FileText } from "lucide-react"
import { Button } from "@/components/ui/button"

export function Footer() {
  return (
    <footer className="border-t border-border/40 bg-background">
      <div className="container flex flex-col items-center justify-between gap-4 md:h-24 md:flex-row max-w-screen-2xl">
        <div className="flex flex-col items-center gap-4 px-8 md:flex-row md:gap-2 md:px-0">
          <p className="text-center text-sm leading-loose text-muted-foreground md:text-left">
            Built for developers, by developers. Complete and secure user authentication for your SaaS or web platform.
          </p>
        </div>
        
        <div className="flex items-center space-x-1">
          <Button variant="ghost" size="sm" asChild>
            <a 
              href="https://github.com/nauth/nauth" 
              target="_blank" 
              rel="noopener noreferrer"
              className="flex items-center gap-2"
            >
              <Github className="h-4 w-4" />
              GitHub
            </a>
          </Button>
          <Button variant="ghost" size="sm" asChild>
            <a 
              href="https://docs.nauth.dev" 
              target="_blank" 
              rel="noopener noreferrer"
              className="flex items-center gap-2"
            >
              <BookOpen className="h-4 w-4" />
              Docs
            </a>
          </Button>
          <Button variant="ghost" size="sm" asChild>
            <a 
              href="https://github.com/nauth/nauth/blob/main/LICENSE" 
              target="_blank" 
              rel="noopener noreferrer"
              className="flex items-center gap-2"
            >
              <FileText className="h-4 w-4" />
              License
            </a>
          </Button>
        </div>
      </div>
    </footer>
  )
}