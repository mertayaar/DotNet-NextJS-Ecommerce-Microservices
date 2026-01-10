import Link from 'next/link'
import { Button } from "@/components/ui/button"

export default function NotFound() {
    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-background text-foreground space-y-4 text-center px-4">
            <h1 className="text-9xl font-bold text-primary tracking-tighter">404</h1>
            <h2 className="text-2xl font-semibold tracking-tight">Page Not Found</h2>
            <p className="text-muted-foreground max-w-[500px]">
                Sorry, we couldn't find the page you're looking for. It might have been moved or deleted.
            </p>
            <div className="pt-4">
                <Link href="/">
                    <Button variant="default" size="lg">
                        Return Home
                    </Button>
                </Link>
            </div>
        </div>
    )
}
